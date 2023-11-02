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

            var sqlquery = "";
            var sqldwquery = "";

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

    }

}
