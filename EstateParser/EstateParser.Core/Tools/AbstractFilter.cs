using System;
using System.Linq;
using System.Collections.Generic;

namespace EstateParser.Core.Tools
{
    public class Predicate<TModel, TRequest>
    {
        public readonly Func<TModel, TRequest, bool> match;
        public readonly Func<TRequest, bool> canApply;

        public Predicate(Func<TModel, TRequest, bool> match, Func<TRequest, bool> canApply)
        {
            this.match = match;
            this.canApply = canApply;
        }
    }

    public abstract class AbstractFilter<TModel, TRequest>
    {
        #region Fields

        private readonly List<Predicate<TModel, TRequest>> mPredicates = new List<Predicate<TModel, TRequest>>();
        private readonly Func<TRequest> mRequstFactory;

        #endregion

        #region Properties

        public bool CanApply
        {
            get
            {
                var request = mRequstFactory();

                return mPredicates.Any(p => p.canApply(request));
            }
        }

        #endregion

        #region Constructor

        protected AbstractFilter(Func<TRequest> factory)
        {
            mRequstFactory = factory;
        }

        #endregion

        #region Methods

        public bool Match(TModel item, TRequest request)
        {
            if (CanApply)
            {
                return mPredicates.Where(p => p.canApply(request))
                    .All(predicate => predicate.match(item, request));
            }

            return false;
        }

        public bool Match(TModel item)
        {
            return Match(item, mRequstFactory());
        }

        protected void Add(Predicate<TModel, TRequest> predicate)
        {
            if (!mPredicates.Contains(predicate))
            {
                mPredicates.Add(predicate);
            }
        }

        protected void Add(Func<TModel, TRequest, bool> match, Func<TRequest, bool> canApply)
        {
            Add(new Predicate<TModel, TRequest>(match, canApply ?? (request => true)));
        }

        #endregion
    }
}
