using Dapper;
using Microsoft.Data.SqlClient;
using SqlUnitTestDemo.Models;

namespace SqlUnitTestDemo.End2EndTests
{
    public class RowCountByYearQuarterTests: IClassFixture<End2EndTestFixture>
    {
        private readonly End2EndTestFixture _fixture;

        public RowCountByYearQuarterTests(End2EndTestFixture fixture) => _fixture = fixture;

        
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
            Assert.Equal(sourcedata.Count(), targetdata.Count());

            //Loop through each row and compare the row count
            foreach (var source in sourcedata)
            {
                var compare = targetdata.Where(x => x.Year == source.Year && x.Quarter == source.Quarter).First();
                Assert.True(source.RowCount == compare.RowCount,$"Source and Target data doesn't match for Year:{ source.Year} Quarter { source.Quarter} Source Count:{ source.RowCount} vs Target Count: { compare.RowCount} .");
            }
        }

        [Fact(DisplayName = "The distinct count of years should match from the delta file")]
        public async Task DistinctCountOfYearOfDeltaFormatShouldMatch()
        {
            //Setup
            var synapseServerlessConnection = new SqlConnection(await _fixture.GetSynapseSqlServerLessTargetConnectionString());

            //This dataset is publicly available in Azure learn https://learn.microsoft.com/en-us/azure/synapse-analytics/sql/query-delta-lake-format
            var sqlquery = "SELECT distinct([year]) FROM OPENROWSET( BULK 'https://sqlondemandstorage.blob.core.windows.net/delta-lake/covid/', FORMAT = 'delta') as rows ORDER By [year];";

            //Act
            var targetdata = await synapseServerlessConnection.QueryAsync<Year>(sqlquery);
            var sortedTargetdata = targetdata.OrderBy(x => x.year);

            //Assert
            Assert.True(sortedTargetdata.First().year == 2019, $"The first year should be 2019 but it's {sortedTargetdata.First().year}.");
            Assert.True(sortedTargetdata.Last().year == 2020, $"The last year should be 2020 but it's {sortedTargetdata.Last().year}.");
        }

    }

}
