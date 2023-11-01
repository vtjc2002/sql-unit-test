using Microsoft.Extensions.Configuration;

namespace SqlUnitTestDemo.End2EndTests
{
    public class End2EndTestFixture: IDisposable
    {

        private readonly IConfiguration _configuration;

        public End2EndTestFixture()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
        }

        /// <summary>
        /// Release any unmanged resources
        /// </summary>
        public void Dispose()
        {
            
        }

        /// <summary>
        /// Get the fixture configuration
        /// </summary>
        public IConfiguration Configuration => _configuration;
    }
}
