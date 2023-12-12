using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

using Microsoft.Extensions.Configuration;


namespace SqlUnitTestDemo.End2EndTests
{
    public class End2EndTestFixture : IDisposable
    {

        private readonly IConfiguration _configuration;
        private readonly SecretClient _secretClient;
        private string? _sqlusername, _sqlpassword, _sqldwusername, _sqldwpassword;

        public End2EndTestFixture()
        {
            //Get configuration file
            _configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // Get Azure Key Vault client
            var kvUri = "https://" + _configuration.GetSection("AzureResources").GetValue<string>("KeyVaultName") + ".vault.azure.net";
            
            // Using Service Principle here because Managed Identity is only supported in Azure DevOps if it's the same tenant.
            // this sample is not using the same azDo tenant as the Azure Resources.
            _secretClient = new SecretClient(new Uri(kvUri), 
                new ClientSecretCredential(tenantId: _configuration.GetSection("AzureResources").GetValue<string>("TenantId"), 
                    clientId: _configuration.GetSection("AzureResources").GetValue<string>("ServicePrincipleClientId"), 
                    clientSecret: _configuration.GetSection("AzureResources").GetValue<string>("ServicePrincipleClientSecret")));
        }

        /// <summary>
        /// Release any unmanged resources
        /// </summary>
        public void Dispose()
        {

        }


        // Gets the sql connection string from the configuration
        public async Task<string> GetSqlConnectionString()
        {
            var sqlconnectionstring = _configuration.GetConnectionString("Sql");

            if (string.IsNullOrEmpty(_sqlusername))
            {
                var sqlusername = await _secretClient.GetSecretAsync("secret-sql-username");
                _sqlusername = sqlusername.Value.Value;
            }

            if (string.IsNullOrEmpty(_sqlpassword))
            {
                var sqlpassword = await _secretClient.GetSecretAsync("secret-sql-password");
                _sqlpassword = sqlpassword.Value.Value;
            }


            sqlconnectionstring = sqlconnectionstring.Replace("##username##", _sqlusername);
            sqlconnectionstring = sqlconnectionstring.Replace("##password##", _sqlpassword);

            return sqlconnectionstring;
        }

        // Gets the synapse dedicated pool connection string from the configuration
        public async Task<string> GetSqlDedicatedPoolConnectionString()
        {
            var sqldwconnectionstring = _configuration.GetConnectionString("SynapseDedicatedPool");

            if (string.IsNullOrEmpty(_sqldwusername))
            {
                var sqldwusername = await _secretClient.GetSecretAsync("secret-azuresynapse-username");
                _sqldwusername = sqldwusername.Value.Value;
            }

            if (string.IsNullOrEmpty(_sqldwpassword))
            {
                var sqldwpassword = await _secretClient.GetSecretAsync("secret-azuresynapse-password");
                _sqldwpassword = sqldwpassword.Value.Value;
            }

            sqldwconnectionstring = sqldwconnectionstring.Replace("##username##", _sqldwusername);
            sqldwconnectionstring = sqldwconnectionstring.Replace("##password##", _sqldwpassword);

            return sqldwconnectionstring;
        }

        // Gets the Synapse Sql Serverless connection string from the configuration
        public async Task<string> GetSynapseSqlServerLessConnectionString()
        {
            var sqldwconnectionstring = _configuration.GetConnectionString("SynapseSqlServerless");

            if (string.IsNullOrEmpty(_sqldwusername))
            {
                var sqldwusername = await _secretClient.GetSecretAsync("secret-azuresynapse-username");
                _sqldwusername = sqldwusername.Value.Value;
            }

            if (string.IsNullOrEmpty(_sqldwpassword))
            {
                var sqldwpassword = await _secretClient.GetSecretAsync("secret-azuresynapse-password");
                _sqldwpassword = sqldwpassword.Value.Value;
            }

            sqldwconnectionstring = sqldwconnectionstring.Replace("##username##", _sqldwusername);
            sqldwconnectionstring = sqldwconnectionstring.Replace("##password##", _sqldwpassword);

            return sqldwconnectionstring;
        }


    }
}
