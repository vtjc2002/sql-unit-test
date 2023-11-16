using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Server;
using SqlUnitTestDemo.Models;
using System;
using System.Formats.Asn1;
using System.Text.RegularExpressions;

namespace SqlUnitTestDemo.End2EndTests
{
    public class RowCountByYearQuarterTests: IClassFixture<End2EndTestFixture>
    {
        private readonly End2EndTestFixture _fixture;

        public RowCountByYearQuarterTests(End2EndTestFixture fixture) => _fixture = fixture;

        /// <summary>
        /// Example of querying sql db as source and sql dw as target and comparing the results
        /// </summary>
        /// <returns></returns>
        [Fact(DisplayName ="End to EndTest Sql year and quarter count equals Sql Dw data")]
        public async Task RowCountByYearShouldMatch()
        {
            //Setup
            var sqlConnection = new SqlConnection(await _fixture.GetSqlSourceConnectionString());
            var sqldwConnection = new SqlConnection(await _fixture.GetSqlDwTargetConnectionString());

            var sqlquery = "SELECT [year],[quarter],sum([count]) as [RowCount] FROM [dbo].[YearQuarterCount] group by [year],[quarter]";
            var sqldwquery = "SELECT [year],[quarter],sum([count]) as [RowCount] FROM [dbo].[ProcessedYearQuarterCount] Group by [year],[quarter]";

            //Act
            var sourcedata = await sqlConnection.QueryAsync<YearQuarterCount>(sqlquery);
            var targetdata = await sqldwConnection.QueryAsync<YearQuarterCount>(sqldwquery);
            
            //Assert

            //The count of rows should be the same
            Assert.Equal(sourcedata.Count(), targetdata.Count());

            //Loop through each row and compare the row count
            foreach (var source in sourcedata)
            {
                var compare = targetdata.Where(x => x.Year == source.Year && x.Quarter == source.Quarter).First();
                Assert.True(source.RowCount == compare.RowCount,$"Source and Target data doesn't match for Year:{ source.Year} Quarter { source.Quarter} Source Count:{ source.RowCount} vs Target Count: { compare.RowCount} .");
            }
        }

        /// <summary>
        /// Example of querying ADLSv2 delta format using Synapse Sql Serverless.
        /// </summary>
        /// <returns></returns>
        [Fact(DisplayName = "The distinct count of years should match from the delta file")]
        public async Task DeltaFormatDistinctCountOfYearOfShouldMatch()
        {
            //Setup
            var synapseServerlessConnection = new SqlConnection(await _fixture.GetSynapseSqlServerLessTargetConnectionString());
            var sourceData = new List<Year> { new Year { year = 2019 }, new Year { year = 2020 } };

            //This dataset is publicly available in Azure learn https://learn.microsoft.com/en-us/azure/synapse-analytics/sql/query-delta-lake-format
            var sqlquery = "SELECT DISTINCT([year]) ";
            sqlquery += "FROM OPENROWSET( BULK 'https://sqlondemandstorage.blob.core.windows.net/delta-lake/covid/', FORMAT = 'delta') ";
            sqlquery += "WITH ([year] smallint) ";
            sqlquery += "AS rows ORDER BY [year];";

            //Act
            var targetData = await synapseServerlessConnection.QueryAsync<Year>(sqlquery);

            //Assert

            //the count should be the same
            Assert.True(sourceData.Count() == targetData.Count(), $"The count of years should be the same but target distinct count is {targetData.Count()}.");

            //the years should be the same
            sourceData.ForEach( source =>
            {
                var compare = targetData.Where(x => x.year == source.year).First();
                Assert.True(source.year == compare.year, $"The source year {source.year} should be the same but target year is {compare.year}.");
            });
        }

        /// <summary>
        /// Example of querying ADLSv2 parquet format using Synapse Sql Serverless.
        /// </summary>
        [Fact(DisplayName = "The sum of death by year should match from the parquet file")]
        public async Task ParquetFormatDeathSumShouldMatch()
        {
            // Setup
            var synapseServerlessConnection = new SqlConnection(await _fixture.GetSynapseSqlServerLessTargetConnectionString());

            // Intentionally using month = 0 to indicate sum.
            var sourceData = new List<CovidDeath>() { new CovidDeath { Year = 2019, Month = 0, Deaths = 0 }, new CovidDeath { Year = 2020, Month = 0, Deaths = 1612833 } };

            //This dataset is publicly available in Azure learn https://learn.microsoft.com/en-us/azure/synapse-analytics/sql/query-parquet-files
            var sqlquery = "SELECT [year], month, sum(deaths) as deaths " +
            "FROM openrowset( BULK 'https://pandemicdatalake.blob.core.windows.net/public/curated/covid-19/ecdc_cases/latest/ecdc_cases.parquet', " +
            "FORMAT = 'parquet') " +
            "WITH([year] smallint, [month] TINYINT, deaths int) " +
            "AS rows " +
            "GROUP BY[year], [month];";

            // Act
            var targetData = await synapseServerlessConnection.QueryAsync<CovidDeath>(sqlquery);

            // here we are grouping the data by year and month and summing the death count. Intentionally using month = 0 to indicate sum.
            var targetDataGrouped = targetData.GroupBy(x => new { x.Year }).Select(x => new CovidDeath { Year = x.Key.Year, Deaths = x.Sum(y => y.Deaths) });


            // Assert
            Assert.NotEmpty(targetData);

            sourceData.ForEach(source =>
            {
                var compare = targetDataGrouped.Where(x => x.Year == source.Year && x.Month == 0).First();
                Assert.True(source.Deaths == compare.Deaths, $"The source death count {source.Deaths} should be the same but target death count is {compare.Deaths}.");
            });

        }

    }

}
