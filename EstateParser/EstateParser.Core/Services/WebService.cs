using System;
using System.IO;
using System.Net.Http;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using EstateParser.Contracts;
using EstateParser.Contracts.Services;
using EstateParser.Core.Tools;

namespace EstateParser.Core.Services
{
    public class WebService : IWebService
    {
        private readonly Pool<HttpClient> mClientPool;
        private readonly ExecutionDataflowBlockOptions mDataflowBlockOptions;

        public WebService(IConfiguration configuration)
        {
            mClientPool = new Pool<HttpClient>();

            mDataflowBlockOptions = new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = configuration.Get("BoundedCapacity").AsInt(),
                MaxDegreeOfParallelism = configuration.Get("MaxDegreeOfParallelism").AsInt()
            };
        }

        public Stream DownloadStream(string url)
        {
            var client = mClientPool.Take();

            try
            {
                return client.GetStreamAsync(url).Result;
            }
            finally
            {
                mClientPool.Release(client);
            }
        }   

        public async Task<Stream> DownloadStreamAsync(string url, CancellationToken cancellationToken)
        {
            var client = mClientPool.Take();

            try
            {
                using (cancellationToken.Register(client.CancelPendingRequests))
                {
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    return await client.GetStreamAsync(url);
                }
            }
            finally
            {
                mClientPool.Release(client);
            }
        }

        public async Task<TResult[]> ExecuteBatchQuery<TInput, TResult>(TInput[] args, Func<TInput, TResult> factory,
            CancellationToken token)
        {
            var list = new ConcurrentBag<TResult>();

            var mTransformBlock = new TransformBlock<TInput, TResult>(factory, mDataflowBlockOptions);
            var actionBlock = new ActionBlock<TResult>(data => list.Add(data), mDataflowBlockOptions);

            mTransformBlock.LinkTo(actionBlock);
            mTransformBlock.Completion.ContinueWith(item => actionBlock.Complete(), token);

            foreach (var arg in args)
                mTransformBlock.Post(arg);

            mTransformBlock.Complete();

            await actionBlock.Completion;

            return list.ToArray();
        }
    }
}
