using System.Configuration;
using EstateParser.Contracts;

namespace EstateParser.Infrastructure
{
    public class AppConfiguration : IConfiguration
    {
        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
