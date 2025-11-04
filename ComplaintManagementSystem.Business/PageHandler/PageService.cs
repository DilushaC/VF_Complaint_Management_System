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

namespace ComplaintManagementSystem.Business.PageHandler
{
    public class PageService : IPageService
    {
        private readonly _ConnectionService _connectionService;

        public PageService(_ConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public List<PageModel> getAllList()
        {

            try
            {
                string Query = $"SELECT * FROM Complaint_Page_Master WHERE Active = 1";
                var Data = _connectionService.Return(Query);
                var Row = Data.Rows[0];

                List<PageModel> List = new List<PageModel>();

                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    PageModel bModel = new PageModel()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        Page = BRow["Page"].ToString(),
                        Active = Convert.ToBoolean(BRow["Active"]),
                        CreatedDate = Convert.ToDateTime(BRow["CreatedDate"]),
                    };
                    List.Add(bModel);
                }
                return List;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Create(IFormCollection collection)
        {

            var Page = collection["Page"].ToString();

            var query = "INSERT INTO Complaint_Page_Master (Page, Active, CreatedDate) " +
                    "VALUES (@name, @active, @createdDate);";

            var parameters = new DynamicParameters();

            parameters.Add("name", Page, DbType.String);
            parameters.Add("active", 1, DbType.Int32);
            parameters.Add("createdDate", System.DateTime.Now, DbType.DateTime);

            _connectionService.ReturnWithPara(query, parameters);

            return;
        }

        public PageModel getListId(int Id)
        {

            string query = $@" SELECT * FROM Complaint_Page_Master WHERE Id={Id} ";
            var data = _connectionService.Return(query);
            var row = data.Rows[0];
            PageModel uRoleList = new PageModel()
            {
                Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
                Page = row["Page"] != DBNull.Value ? row["Page"].ToString() : string.Empty,
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
                var Name = collection["Pages"].ToString();
                string query = @"
                                UPDATE Complaint_Page_Master
                                SET 
                                    Page = @name
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
                                UPDATE Complaint_Page_Master
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
