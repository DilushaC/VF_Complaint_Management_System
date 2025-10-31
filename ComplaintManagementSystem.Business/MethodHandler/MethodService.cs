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

namespace ComplaintManagementSystem.Business.MethodHandler
{
    public class MethodService : IMethodService
    {
        private readonly _ConnectionService _connectionService;

        public MethodService(_ConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public List<Complaint_Method_MasterModel> getAllList()
        {
            try
            {
                string Query = $"SELECT * FROM Complaint_Method_Master WHERE Active = 1";
                var Data = _connectionService.Return(Query);
                List<Complaint_Method_MasterModel> depList = new List<Complaint_Method_MasterModel>();

                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    Complaint_Method_MasterModel methodModel = new Complaint_Method_MasterModel()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        Method = BRow["Method"].ToString(),
                        Code = BRow["Code"].ToString(),
                        Active = Convert.ToBoolean(BRow["Active"]),
                        CreatedDate = Convert.ToDateTime(BRow["CreatedDate"]),
                    };
                    depList.Add(methodModel);
                }
                return depList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateMethod(IFormCollection collection)
        {

            var Name = collection["Method"].ToString();
            var Code = collection["Code"].ToString();

            var query = "INSERT INTO Complaint_Method_Master (Method, Code, Active, CreatedDate) " +
                    "VALUES (@name, @code, @active, @createdDate);";

            var parameters = new DynamicParameters();

            parameters.Add("name", Name, DbType.String);
            parameters.Add("code", Code, DbType.String);
            parameters.Add("active", 1, DbType.Int32);
            parameters.Add("createdDate", System.DateTime.Now, DbType.DateTime);

            _connectionService.ReturnWithPara(query, parameters);

            return;
        }

        public Complaint_Method_MasterModel getMethodListId(int Id)
        {

            string query = $@" SELECT * FROM Complaint_Method_Master WHERE Id={Id} ";
            var data = _connectionService.Return(query);
            var row = data.Rows[0];
            Complaint_Method_MasterModel mList = new Complaint_Method_MasterModel()
            {
                Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
                Method = row["Method"] != DBNull.Value ? row["Method"].ToString() : string.Empty,
                Code = row["Code"] != DBNull.Value ? row["Code"].ToString() : string.Empty,
                Active = row["Active"] != DBNull.Value && Convert.ToBoolean(row["Active"]),
                CreatedDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.MinValue
            };
            return mList;
        }

        public void UpdateMethod(IFormCollection collection)
        {
            try
            {
                var id = collection["Id"].ToString();
                var Name = collection["Methods"].ToString();
                var Code = collection["Codes"].ToString();

                string query = @"
                                UPDATE Complaint_Method_Master
                                SET 
                                    Method = @name,
                                    Code = @code
                                WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Convert.ToInt64(id), DbType.Int64);
                parameters.Add("@name", Name, DbType.String);
                parameters.Add("@Code", Code, DbType.String);

                _connectionService.ExecuteWithPara(query, parameters);
                return;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void DeleteMethod(int id)
        {
            try
            {
                string query = @"
                                UPDATE Complaint_Method_Master
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
