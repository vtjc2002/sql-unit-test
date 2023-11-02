using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

using Microsoft.Extensions.Configuration;


namespace SqlUnitTestDemo.End2EndTests
{
    public class End2EndTestFixture : IDisposable
    {

        private readonly IConfiguration _configuration;
        private readonly SecretClient _secretClient;
        private string _sqlusername, _sqlpassword, _sqldwusername, _sqldwpassword;

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

            if (string.IsNullOrEmpty(_sqlusername))
            {
                var sqlusername = await _secretClient.GetSecretAsync("secret-sql-username-source");
                _sqlusername = sqlusername.Value.Value;
            }

            if (string.IsNullOrEmpty(_sqlpassword))
            {
                var sqlpassword = await _secretClient.GetSecretAsync("secret-sql-password-source");
                _sqlpassword = sqlpassword.Value.Value;
            }


            sqlconnectionstring = sqlconnectionstring.Replace("##username##", _sqlusername);
            sqlconnectionstring = sqlconnectionstring.Replace("##password##", _sqlpassword);

            return sqlconnectionstring;
        }

        // Gets the sql dw target connection string from the configuration
        public async Task<string> GetSqlDwTargetConnectionString()
        {
            var sqldwconnectionstring = _configuration.GetConnectionString("SqlDwTarget");

            if (string.IsNullOrEmpty(_sqldwusername))
            {
                var sqldwusername = await _secretClient.GetSecretAsync("secret-sqldw-username-target");
                _sqldwusername = sqldwusername.Value.Value;
            }

            if (string.IsNullOrEmpty(_sqldwpassword))
            {
                var sqldwpassword = await _secretClient.GetSecretAsync("secret-sqldw-password-target");
                _sqldwpassword = sqldwpassword.Value.Value;
            }

            sqldwconnectionstring = sqldwconnectionstring.Replace("##username##", _sqldwusername);
            sqldwconnectionstring = sqldwconnectionstring.Replace("##password##", _sqldwpassword);

            return sqldwconnectionstring;
        }


    }
}
