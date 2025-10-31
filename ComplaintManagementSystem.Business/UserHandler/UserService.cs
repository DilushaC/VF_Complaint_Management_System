using ComplaignManagementSystem.Data.Context;
using ComplaignManagementSystem.Data.Models;
using ComplaintManagementSystem.Business.ConncetionHandler;
using ComplaintManagementSystem.Business.Helpers;
using Dapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ComplaintManagementSystem.Business.LoginHandler
{
    public class UserService : IUserService
    {
        private readonly _ConnectionService _connectionService;
        private readonly PasswordHelper _passwordHelper;

        public UserService(_ConnectionService connectionService, PasswordHelper passwordHelper)
        {
            _connectionService = connectionService;
            _passwordHelper = passwordHelper;
        }

        public async Task<UserModel> ValidateUserAsync(string username, string password)
        {
            const string query = @"SELECT * FROM Complaint_User WHERE UserName = @UserName AND Active = 1";

            var parameters = new DynamicParameters();
            parameters.Add("@UserName", username);

            // Use the centralized connection handler for DB access
            var users = _connectionService.ReturnWithPara(query, parameters)
                                          .AsEnumerable()
                                          .Select(row => new UserModel
                                          {
                                              Id = row.Field<int>("Id"),
                                              UserName = row.Field<string>("UserName"),
                                              Password = row.Field<string>("Password"),
                                              SaltKey = row.Field<string>("SaltKey"),
                                              Name = row.Field<string>("Name"),
                                              Email = row.Field<string>("Email"),
                                              BranchId = row.Field<int?>("BranchId"),
                                              Dep_Id = row.Field<int?>("Dep_Id"),
                                              CreatedDate = row.Field<System.DateTime>("CreatedDate"),
                                              Active = row.Field<bool>("Active"),
                                              IsReset = row.Field<bool>("IsReset")
                                          })
                                          .ToList();

            var user = users.FirstOrDefault();
            if (user == null)
                return null;

            bool isValid = _passwordHelper.VerifyPassword(password, user.Password);
            return isValid ? user : null;
        }

        public async Task ResetPassword(string userId, string saltKey, string NewPassword)
        {
            try
            {
                string EncryptNewPassword = _passwordHelper.ComputeHmac(NewPassword);
                string query = $@"UPDATE Complaint_User SET IsReset=1, Password=@NewPAssword WHERE Id=@Id";
                //string query = $@" UPDATE Complaint_ManageProcess SET IsSentCentral = 1, IsSentCentralDateTime = @IsSentCentralDateTime WHERE Id={Id} ";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Convert.ToInt32(userId), DbType.Int64);
                parameters.Add("@NewPAssword", EncryptNewPassword, DbType.String);

                _connectionService.ExecuteWithPara(query, parameters);
                //_connectionService.Return(query);
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UserPermissionModel getAccessPerimissions(UserModel user)
        {
            try
            {
                string query = @"
                                SELECT 
                                    UP.Id AS Id,
                                    CU.Id AS UserId,
                                    UR.Id AS UserRoleId,
                                    UR.Role AS Role
                                FROM Complaint_User AS CU
                                INNER JOIN Complaint_User_Permission AS UP ON UP.UserId = CU.Id
                                INNER JOIN Complaint_User_Role AS UR ON UR.Id = UP.UserRoleId
                                WHERE CU.Active = 1 AND UP.Active = 1 AND CU.Id = @UserId";

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", user.Id);

                var data = _connectionService.ReturnWithPara(query, parameters);
                var Row = data.Rows[0];

                UserPermissionModel uPermissionModel = new UserPermissionModel()
                {
                    Id = Convert.ToInt32(Row["Id"]),
                    UserId = Convert.ToInt32(Row["UserId"]),
                    UserRoleId = Convert.ToInt32(Row["UserRoleId"]),
                    Role = Row["Role"].ToString(),
                };

                return uPermissionModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<UserPageCapabilityModel> getAccessPages(UserModel user, UserPermissionModel uPermission)
        {
            try
            {
                string query = @"SELECT 
                                UR.Id AS Id,
                                UR.Role AS Role,
                                CPM.Id AS PageId,
                                CPM.Page AS Page,
                                UPC.IsEdit AS IsEdit,
                                UPC.Active AS Active
                            FROM Complaint_User_Role AS UR
                            INNER JOIN Complaint_User_PageCapability AS UPC ON UPC.UserRoleId = UR.Id
                            INNER JOIN Complaint_Page_Master AS CPM ON CPM.Id = UPC.PageId
                            WHERE UR.Active=1 AND CPM.Active=1 AND UR.Id = @UserRoleId";
                var parameters = new DynamicParameters();
                parameters.Add("@UserRoleId", uPermission.UserRoleId);

                var data = _connectionService.ReturnWithPara(query, parameters);

                List<UserPageCapabilityModel> pageList = new List<UserPageCapabilityModel>();
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    UserPageCapabilityModel pageAccessModel = new UserPageCapabilityModel();
                    var Row = data.Rows[i];
                    pageAccessModel.UserRoleId = Convert.ToInt32(Row["Id"]);
                    pageAccessModel.UserRole = Row["Role"].ToString();
                    pageAccessModel.PageId = Convert.ToInt32(Row["PageId"]);
                    pageAccessModel.Page = Row["Page"].ToString();
                    pageAccessModel.IsEdit = Convert.ToBoolean(Row["IsEdit"]);
                    pageAccessModel.Active = Convert.ToBoolean(Row["Active"]);
                        //Role = Row["Role"].ToString(),
                    
                    pageList.Add(pageAccessModel);
                }
                return pageList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DepartmentModel GetDepartmentDetails(int DepId)
        {
            try
            {
                const string query = @"SELECT * FROM Complaint_Department_Master WHERE Id = @Id AND Active = 1";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", DepId);

                // Use the centralized connection handler for DB access
                var departments = _connectionService.ReturnWithPara(query, parameters)
                                              .AsEnumerable()
                                              .Select(row => new DepartmentModel
                                              {
                                                  Id = row.Field<int>("Id"),
                                                  Name = row.Field<string>("Name"),
                                                  Code = row.Field<string>("Code"),
                                                  CreatedDate = row.Field<System.DateTime>("CreatedDate"),
                                                  Active = row.Field<bool>("Active"),
                                                  Status = row.Field<bool>("Status")
                                              })
                                              .ToList();

                var department = departments.FirstOrDefault();
                if (department == null)
                    return null;

                return department;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}

