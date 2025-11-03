using ComplaignManagementSystem.Data.Models;
using ComplaintManagementSystem.Business.ConncetionHandler;
using Dapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.UserRoleHandler
{
    public class UserRoleService : IUserRoleService
    {
        private readonly _ConnectionService _connectionService;

        public UserRoleService(_ConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public List<UserRoleModel> getAllList()
        {

            try
            {
                string Query = $"SELECT * FROM Complaint_User_Role WHERE Active = 1";
                var Data = _connectionService.Return(Query);
                var Row = Data.Rows[0];

                List<UserRoleModel> uRoleList = new List<UserRoleModel>();

                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    UserRoleModel bModel = new UserRoleModel()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        Role = BRow["Role"].ToString(),
                        Active = Convert.ToBoolean(BRow["Active"]),
                        CreatedDate = Convert.ToDateTime(BRow["CreatedDate"]),                        
                    };
                    uRoleList.Add(bModel);
                }
                return uRoleList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Create(IFormCollection collection)
        {

            var Name = collection["Role"].ToString();

            var query = "INSERT INTO Complaint_User_Role (Role, Active, CreatedDate) " +
                    "VALUES (@name, @active, @createdDate);";

            var parameters = new DynamicParameters();

            parameters.Add("name", Name, DbType.String);
            parameters.Add("active", 1, DbType.Int32);
            parameters.Add("createdDate", System.DateTime.Now, DbType.DateTime);

            _connectionService.ReturnWithPara(query, parameters);

            return;
        }

        public UserRoleModel getListId(int Id)
        {

            string query = $@" SELECT * FROM Complaint_User_Role WHERE Id={Id} ";
            var data = _connectionService.Return(query);
            var row = data.Rows[0];
            UserRoleModel uRoleList = new UserRoleModel()
            {
                Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
                Role = row["Role"] != DBNull.Value ? row["Role"].ToString() : string.Empty,
                Active = row["Active"] != DBNull.Value && Convert.ToBoolean(row["Active"]),
                CreatedDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.MinValue
            };
            return uRoleList;
        }

        public void Update(IFormCollection collection)
        {
            try
            {
                var id = collection["Id"].ToString();
                var Name = collection["Roles"].ToString();
                string query = @"
                                UPDATE Complaint_User_Role
                                SET 
                                    Role = @name
                                WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Convert.ToInt64(id), DbType.Int64);
                parameters.Add("@name", Name, DbType.String);

                _connectionService.ExecuteWithPara(query, parameters);
                return;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Delete(int id)
        {
            try
            {
                string query = @"
                                UPDATE Complaint_User_Role
                                SET Active = 0
                                WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Convert.ToInt64(id), DbType.Int64);

                _connectionService.ExecuteWithPara(query, parameters);
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
