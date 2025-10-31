using ComplaignManagementSystem.Data.Models;
using ComplaintManagementSystem.Business.ConncetionHandler;
using ComplaintManagementSystem.Business.Helpers;
using Dapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.DepartmentHandler
{
    public class DepartmentService : IDepartmentService
    {
        private readonly _ConnectionService _connectionService;

        public DepartmentService(_ConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public List<DepartmentModel> getAllList()
        {

            try
            {
                string Query = $"SELECT * FROM Complaint_Department_Master WHERE Active = 1";
                var Data = _connectionService.Return(Query);
                var Row = Data.Rows[0];

                List<DepartmentModel> depList = new List<DepartmentModel>();

                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    DepartmentModel bModel = new DepartmentModel()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        Name = BRow["Name"].ToString(),
                        Code = BRow["Code"].ToString(),
                        Active = Convert.ToBoolean(BRow["Active"]),
                        CreatedDate = Convert.ToDateTime(BRow["CreatedDate"]),
                        Status = Convert.ToBoolean(BRow["Status"]),
                    };
                    depList.Add(bModel);
                }
                return depList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateDepartment(IFormCollection collection)
        {

            var Name = collection["Dep_Name"].ToString();
            var Code = collection["Dep_Code"].ToString();
            var Status = collection["Status"].ToString();

            var query = "INSERT INTO Complaint_Department_Master (Name, Code, Active, CreatedDate, Status) " +
                    "VALUES (@name, @code, @active, @createdDate , @status);";

            var parameters = new DynamicParameters();

            parameters.Add("name", Name, DbType.String);
            parameters.Add("code", Code, DbType.String);
            parameters.Add("active", 1, DbType.Int32);
            parameters.Add("createdDate", System.DateTime.Now, DbType.DateTime);
            parameters.Add("status", Convert.ToInt32(Status), DbType.Int32);

            _connectionService.ReturnWithPara(query, parameters);

            return;
        }

        public DepartmentModel getDepListId(int Id)
        {

            string query = $@" SELECT * FROM Complaint_Department_Master WHERE Id={Id} ";
            var data = _connectionService.Return(query);
            var row = data.Rows[0];
            DepartmentModel depList = new DepartmentModel()
            {
                Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
                Name = row["Name"] != DBNull.Value ? row["Name"].ToString() : string.Empty,
                Code = row["Code"] != DBNull.Value ? row["Code"].ToString() : string.Empty,
                Active = row["Active"] != DBNull.Value && Convert.ToBoolean(row["Active"]),
                Status = row["Status"] != DBNull.Value && Convert.ToBoolean(row["Status"]),
                CreatedDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.MinValue
            };
            return depList;
        }

        public void UpdateDepartment(IFormCollection collection)
        {
            try
            {
                var id = collection["Id"].ToString();
                var Name = collection["Dep_Names"].ToString();
                var Code = collection["Dep_Codes"].ToString();
                var Status = collection["Status"].ToString();
                string query = @"
                                UPDATE Complaint_Department_Master
                                SET 
                                    Name = @name,
                                    Code = @code,
                                    Status = @status
                                WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Convert.ToInt64(id), DbType.Int64);
                parameters.Add("@name", Name, DbType.String);
                parameters.Add("@Code", Code, DbType.String);
                parameters.Add("@Status", Status, DbType.String);

                _connectionService.ExecuteWithPara(query, parameters);
                return;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void DeleteDepartment(int id)
        {
            try
            {
                string query = @"
                                UPDATE Complaint_Department_Master
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
