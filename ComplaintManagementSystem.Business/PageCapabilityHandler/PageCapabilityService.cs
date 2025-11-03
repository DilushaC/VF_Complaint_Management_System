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

namespace ComplaintManagementSystem.Business.PageCapabilityHandler
{
    public class PageCapabilityService : IPageCapabilityService
    {
        private readonly _ConnectionService _connectionService;

        public PageCapabilityService(_ConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public List<PageCapabilityModel> getAllList()
        {
            try
            {
                string Query = @"
                            SELECT PC.Id AS Id,
                                UR.Role AS UserRole, 
                                UR.Id AS UserRoleId, 
                                PM.Page AS Page, 
                                PM.Id AS PageId, 
                                PC.IsEdit AS IsEdit, 
                                PC.CreatedDate AS CreatedDate 
                            FROM Complaint_User_PageCapability AS PC
                            INNER JOIN Complaint_User_Role AS UR ON UR.Id = PC.UserRoleId
                            INNER JOIN Complaint_Page_Master AS PM ON PM.Id = PC.PageId
                            WHERE PC.Active=1";
                var Data = _connectionService.Return(Query);
                var Row = Data.Rows[0];

                List<PageCapabilityModel> List = new List<PageCapabilityModel>();

                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    PageCapabilityModel bModel = new PageCapabilityModel()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        UserRole = BRow["UserRole"].ToString(),
                        UserRoleId = Convert.ToInt32(BRow["UserRoleId"]),
                        Page = BRow["Page"].ToString(),
                        PageId = Convert.ToInt32(BRow["PageId"]),
                        IsEdit = Convert.ToBoolean(BRow["IsEdit"]),
                        CreatedDate = Convert.ToDateTime(BRow["createdDate"]),
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

        public async Task<List<UserRoleModel>> getUserRoleList()
        {
            try
            {
                string Query = $"SELECT * FROM Complaint_User_Role WHERE Active=1";
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
                return uRoleList.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<PageModel>> getPageList()
        {
            try
            {
                string Query = $"SELECT * FROM Complaint_Page_Master WHERE Active=1";
                var Data = _connectionService.Return(Query);
                var Row = Data.Rows[0];

                List<PageModel> uRoleList = new List<PageModel>();

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
                    uRoleList.Add(bModel);
                }
                return uRoleList.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Create(IFormCollection collection)
        {
            try
            {
                var UserRoleId = collection["UserRoleId"].ToString();
                var PageId = collection["PageIds"].ToString();
                var IsEdit = collection["IsEdit"].ToString();

                var query = "INSERT INTO Complaint_User_PageCapability (UserRoleId, PageId, IsEdit, Active, CreatedDate) " +
                        "VALUES (@uRoleId, @pageId, @isEdit, @active , @createdDate);";

                var parameters = new DynamicParameters();

                parameters.Add("uRoleId", Convert.ToInt64(UserRoleId), DbType.Int64);
                parameters.Add("pageId", Convert.ToInt64(PageId), DbType.Int64);
                parameters.Add("isEdit", Convert.ToInt32(IsEdit), DbType.Int32);
                parameters.Add("active", 1, DbType.Int32);
                parameters.Add("createdDate", System.DateTime.Now, DbType.DateTime);

                _connectionService.ReturnWithPara(query, parameters);

                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckAvailability(int roleId, int pageId)
        {
            string query = $@" SELECT * FROM Complaint_User_PageCapability WHERE UserRoleId={roleId} AND PageId={pageId} AND Active=1 ";
            var data = _connectionService.Return(query);
            var row = data.Rows;
            if (data.Rows.Count > 0) { return true; }
            else { return false; }
        }


        public PageCapabilityModel getPageCapListId(int Id)
        {
            try
            {
                string query = $@"SELECT PC.Id AS Id,
                                UR.Role AS UserRole, 
                                UR.Id AS UserRoleId, 
                                PM.Page AS Page, 
                                PM.Id AS PageId, 
                                PC.IsEdit AS IsEdit, 
                                PC.CreatedDate AS CreatedDate 
                            FROM Complaint_User_PageCapability AS PC
                            INNER JOIN Complaint_User_Role AS UR ON UR.Id = PC.UserRoleId
                            INNER JOIN Complaint_Page_Master AS PM ON PM.Id = PC.PageId
                            WHERE PC.Id={Id} ";
                var data = _connectionService.Return(query);
                var row = data.Rows[0];
                PageCapabilityModel model = new PageCapabilityModel()
                {
                    Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
                    UserRoleId = row["UserRoleId"] != DBNull.Value ? Convert.ToInt32(row["UserRoleId"]) : 0,
                    UserRole = row["UserRole"] != DBNull.Value ? row["UserRole"].ToString() : string.Empty,
                    PageId = row["PageId"] != DBNull.Value ? Convert.ToInt32(row["PageId"]) : 0,
                    Page = row["Page"] != DBNull.Value ? row["Page"].ToString() : string.Empty,
                    IsEdit = row["IsEdit"] != DBNull.Value && Convert.ToBoolean(row["IsEdit"]),
                    CreatedDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.MinValue
                };
                return model;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Update(IFormCollection collection)
        {
            try
            {
                var id = collection["Id"].ToString();
                var UserRoleId = collection["UserRoleIds"].ToString();
                var PageId = collection["PageIds"].ToString();
                var IsEdit = collection["IsEdits"].ToString();
                string query = @"
                                UPDATE Complaint_User_PageCapability
                                SET 
                                    UserRoleId = @uRoleId,
                                    PageId = @pageId,
                                    IsEdit = @isEdit
                                WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Convert.ToInt64(id), DbType.Int64);
                parameters.Add("@uRoleId", Convert.ToInt64(UserRoleId), DbType.Int64);
                parameters.Add("@pageId", Convert.ToInt64(PageId), DbType.Int64);
                parameters.Add("@isEdit", Convert.ToInt32(IsEdit), DbType.Int32);

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
                                UPDATE Complaint_User_PageCapability
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
