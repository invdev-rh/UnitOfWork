using Microsoft.Extensions.Configuration;

namespace Uow.Data.Core
{
    public class SampleDbSettings : IDbSettings
    {
        private readonly IConfigurationRoot _configurationRoot;

        public SampleDbSettings(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

        public string ConnectionString()
        {
            return _configurationRoot.GetConnectionString("SampleDb");
        }
    }

    public interface IDbSettings
    {
        string ConnectionString();
    }
}
