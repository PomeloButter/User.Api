using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace Project.API.Applications.Queries
{
    public class ProjectQueries : IProjectQueries
    {
        private readonly string _connstring;

        public ProjectQueries(string connstring)
        {
            _connstring = connstring;
        }

        public async Task<dynamic> GetProjectByUserId(int userId)
        {
            using (var conn = new MySqlConnection(_connstring))
            {
                conn.Open();
                var sql =
                    @"SELECT Projects.Id,projects.
                    Avator,Projects.Company,Projects.FinStage,
                    Projects.Introduction,Projects.ShowSecurityInfo,
                    Projects.CreateTime FROM Projects 
                    WHERE Projects.UserId=@userId";
                var result = await conn.QueryAsync<dynamic>(sql, new {userId});
                return result;
            }
        }

        public async Task<dynamic> GetProjectDetail(int projectId)
        {
            using (var conn = new MySqlConnection(_connstring))
            {
                conn.Open();
                var sql = @"SELECT 
                           projects.Company,
                           projects.CityName,
                           projects.ProvinceName,
                           projects.FinStage,
                           projects.FinMoney,
                           projects.Valuation,
                           projects.FinPercentag,
                           projects.Introduction,
                           projects.UserId,
                           projects.Income,
                           projects.Revenue,
                           projects.Avator,
                           projects.BrokerageOptions,
                           projectvisablerule.Tags,
                           projectvisablerule.Visable
                           FROM projects 
                           INNER JOIN projectvisablerule  
                           on projects.Id=projectvisablerule.ProjectId where Projects.Id=@projectId";
                var result = await conn.QueryAsync<dynamic>(sql, new {projectId});
                return result;
            }
        }
    }
}