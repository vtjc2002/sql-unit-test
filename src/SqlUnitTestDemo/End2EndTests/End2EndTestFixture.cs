using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

using Microsoft.Extensions.Configuration;



namespace SqlUnitTestDemo.End2EndTests
{
    public class End2EndTestFixture: IDisposable
    {

        private readonly IConfiguration _configuration;
        private readonly SecretClient _secretClient;

        public End2EndTestFixture()
        {
            //Get configuration file
            _configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // Get Azure Key Vault client
            var kvUri = "https://" + _configuration.GetSection("AzureResources").GetValue<string>("AzureKeyVaultName") + ".vault.azure.net";
            _secretClient = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
        }

        /// <summary>
        /// Release any unmanged resources
        /// </summary>
        public void Dispose()
        {
            
        }


        // Gets the sql source connection string from the configuration
        public async Task<string> GetSqlSourceConnectionString()
        {
            var sqlconnectionstring = _configuration.GetConnectionString("SqlSource");
            var sqlusername = await _secretClient.GetSecretAsync("secret-sql-username-source");
            var sqlpassword = await _secretClient.GetSecretAsync("secret-sql-password-source");


            sqlconnectionstring.Replace("##username##", sqlusername.Value.Value);
            sqlconnectionstring.Replace("##password##", sqlpassword.Value.Value);

            return sqlconnectionstring;
        }

        // Gets the sql dw target connection string from the configuration
        public async Task<string> GetSqlDwTargetConnectionString()
        {
            var sqldwconnectionstring= _configuration.GetConnectionString("SqlSource");
            var sqlusername = await _secretClient.GetSecretAsync("secret-sqldw-username-target");
            var sqlpassword = await _secretClient.GetSecretAsync("secret-sqldw-password-target");


            sqldwconnectionstring.Replace("##username##", sqlusername.Value.Value);
            sqldwconnectionstring.Replace("##password##", sqlpassword.Value.Value);

            return sqldwconnectionstring;
        }
    }
}
