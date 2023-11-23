﻿using SqlUnitTestDemo.Enums;

namespace SqlUnitTestDemo.End2EndTests
{
    public class ParameterizedTestSamples : IClassFixture<End2EndTestFixture>
    {
        private readonly End2EndTestFixture _fixture;

        public ParameterizedTestSamples(End2EndTestFixture fixture) => _fixture = fixture;

        /// <summary>
        /// Example of a parameterized test using the TestDataGenerator class
        /// </summary>
        /// <param name="testName"></param>
        /// <param name="source"></param>
        /// <param name="sourceQuery"></param>
        /// <param name="target"></param>
        /// <param name="targetQuery"></param>
        /// <param name="Assertion"></param>
        [Theory]
        [ClassData(typeof(TestData.TestDataGenerator))]
        public async Task ReadFromTestDataSample(string testName, DataSource source, string sourceQuery, DataSource target, string targetQuery, string Assertion)
        {
            //Setup
            string sourceConnectionString = await GetSourceConnectionString(source);
            string targetConnectionString = await GetSourceConnectionString(target);

            //Act

            

            //Assert
            Assert.True(!string.IsNullOrEmpty(testName),"Test name should not be empty.");

        }

        /// <summary>
        /// private method to get the connection string for the data source type
        /// </summary>
        private async Task<string> GetSourceConnectionString(DataSource dataSourceType)
        {
            string connectionString;

            if (dataSourceType == DataSource.sql)
            {
                connectionString = await _fixture.GetSqlConnectionString();
            }
            else if (dataSourceType == DataSource.azureSynapseDedicatedPool)
            {
                connectionString = await _fixture.GetSqlDedicatedPoolConnectionString();
            }
            else if (dataSourceType == DataSource.azureSynapseServerless)
            {
                connectionString = await _fixture.GetSynapseSqlServerLessConnectionString();
            }
            else
            {
                connectionString = await _fixture.GetSqlConnectionString();
            }

            return connectionString;
        }

    }
}