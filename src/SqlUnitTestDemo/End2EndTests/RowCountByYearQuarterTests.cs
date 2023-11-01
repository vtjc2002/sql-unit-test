using Dapper;
using Microsoft.Data.SqlClient;
using SqlUnitTestDemo.Models;

namespace SqlUnitTestDemo.End2EndTests
{
    public class RowCountByYearQuarterTests: IClassFixture<End2EndTestFixture>
    {
        private readonly End2EndTestFixture _fixture;

        public RowCountByYearQuarterTests(End2EndTestFixture fixture) => _fixture = fixture;

        
        [Fact]
        public async Task RowCountByYearShouldMatch()
        {
            //Setup
            var sqlConnection = new SqlConnection(await _fixture.GetSqlSourceConnectionString());
            var sqldwConnection = new SqlConnection(await _fixture.GetSqlDwTargetConnectionString());

            //Act
            var sourcedata = await sqlConnection.QueryAsync<YearQuarterCount>("SELECT Year, Quarter, RowCount FROM dbo.RowCountByYearQuarter ORDER BY Year, Quarter");
            var targetdata = sqldwConnection.QueryAsync<YearQuarterCount>("SELECT Year, Quarter, RowCount FROM dbo.RowCountByYearQuarter ORDER BY Year, Quarter");
            
            //Assert
        }
    }

}
