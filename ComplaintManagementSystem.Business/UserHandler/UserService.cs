using ComplaignManagementSystem.Data.Context;
using ComplaignManagementSystem.Data.Models;
using ComplaintManagementSystem.Business.Authentication;
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
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ComplaintManagementSystem.Business.LoginHandler
{
    public class UserService : IUserService
    {
        private readonly _ConnectionService _connectionService;
        private readonly PasswordHelper _passwordHelper;
        private readonly ADAuthentication _aDAuthentication;

        public UserService(_ConnectionService connectionService, PasswordHelper passwordHelper, ADAuthentication aDAuthentication)
        {
            _connectionService = connectionService;
            _passwordHelper = passwordHelper;
            _aDAuthentication = aDAuthentication;
        }

        public async Task<UserModel> ValidateUserAsync(string username, string password)
        {
            var response = await _aDAuthentication.AuthenticatewithAD(username, password);
            if (response.Status == true)
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
                                                  //Password = row.Field<string>("Password"),
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

                //bool isValid = _passwordHelper.VerifyPassword(password, user.Password);
                return user;
            }
            else
            {
                return null;
            }

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

        public List<UserModel> getAllList()
        {
            try
            {
                var query = @"
                            SELECT CU.Id AS Id, 
                            CU.UserName AS UserName, 
                            CU.Name AS Name, 
                            CU.Email AS Email, 
                            CBM.Id AS BranchId, 
                            CBM.Branch AS Branch, 
                            CDM.Id as Dep_Id, 
                            CDM.Name AS Department , 
                            CU.CreatedDate AS CreatedDate, 
                            CU.IsReset AS IsReset,
                            CU.Active AS Active
                        FROM Complaint_User AS CU 
                        INNER JOIN Complaint_Branch_Master as CBM ON CBM.Id = CU.BranchId
                        INNER JOIN Complaint_Department_Master AS CDM ON CDM.Id = CU.Dep_Id";
                var Data = _connectionService.Return(query);
                var Row = Data.Rows[0];

                List<UserModel> uList = new List<UserModel>();

                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    UserModel bModel = new UserModel()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        UserName = BRow["UserName"].ToString(),
                        Name = BRow["Name"].ToString(),
                        Email = BRow["Email"].ToString(),
                        Branch = BRow["Branch"].ToString(),
                        Department = BRow["Department"].ToString(),
                        Active = Convert.ToBoolean(BRow["Active"]),
                        CreatedDate = Convert.ToDateTime(BRow["CreatedDate"]),
                    };
                    uList.Add(bModel);
                }
                return uList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<Complaint_Department_MasterModel>> getDepList()
        {
            try
            {
                string Query = $"SELECT * FROM Complaint_Department_Master WHERE Active=1";
                var Data = _connectionService.Return(Query);
                var Row = Data.Rows[0];

                List<Complaint_Department_MasterModel> depList = new List<Complaint_Department_MasterModel>();

                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    Complaint_Department_MasterModel bModel = new Complaint_Department_MasterModel()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        Name = BRow["Name"].ToString(),
                        Code = BRow["Code"].ToString(),
                        Active = Convert.ToBoolean(BRow["Active"]),
                        CreatedDate = Convert.ToDateTime(BRow["CreatedDate"]),
                    };
                    depList.Add(bModel);
                }
                return depList.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<BranchModel>> getBranchList()
        {
            try
            {
                string Query = $"SELECT * FROM Complaint_Branch_Master WHERE Active=1";
                var Data = _connectionService.Return(Query);
                var Row = Data.Rows[0];

                List<BranchModel> List = new List<BranchModel>();
                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    BranchModel bModel = new BranchModel()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        Branch = BRow["Branch"].ToString(),
                        Code = BRow["Code"].ToString(),
                        Active = Convert.ToBoolean(BRow["Active"]),
                        CreatedDate = Convert.ToDateTime(BRow["CreatedDate"]),
                    };
                    List.Add(bModel);
                }
                return List.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateUser(IFormCollection collection)
        {
            try
            {

                var UserName = collection["UserName"].ToString();
                var Name = collection["Name"].ToString();
                var Email = collection["Email"].ToString();
                var BranchId = collection["BranchId"].ToString();
                var Dep_Id = collection["Dep_Id"].ToString();
                var Password = collection["Password"].ToString();
                string EncryptNewPassword = _passwordHelper.ComputeHmac(Password);
                var query = "INSERT INTO Complaint_User (UserName, Password, Name, Email, BranchId, Dep_Id, CreatedDate, Active, IsReset) " +
                    "VALUES (@userName, @password, @name, @email, @branchId, @depId, @createdDate, @active, @isReset)";

                var parameters = new DynamicParameters();

                parameters.Add("userName", UserName, DbType.String);
                parameters.Add("password", EncryptNewPassword, DbType.String);
                parameters.Add("name", Name, DbType.String);
                parameters.Add("email", Email, DbType.String);
                parameters.Add("branchId", Convert.ToInt64(BranchId), DbType.Int64);
                parameters.Add("depId", Convert.ToInt64(Dep_Id), DbType.Int64);
                parameters.Add("createdDate", System.DateTime.Now, DbType.DateTime);
                parameters.Add("active", 1, DbType.Int32);
                parameters.Add("isReset", 1, DbType.Int32);

                _connectionService.ReturnWithPara(query, parameters);
                //Handle file upload
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserModel> getUserDetailId(int Id)
        {
            try
            {
                string query = @"
                                SELECT CU.Id AS Id, 
                                    CU.UserName AS UserName, 
                                    CU.Name AS Name, 
                                    CU.Email AS Email, 
                                    CBM.Id AS BranchId, 
                                    CBM.Branch AS Branch, 
                                    CDM.Id as Dep_Id, 
                                    CDM.Name AS Department , 
                                    CU.CreatedDate AS CreatedDate, 
                                    CU.IsReset AS IsReset,
                                    CU.Active AS Active,   
                                    CU.Password as Password
                                FROM Complaint_User AS CU 
                                INNER JOIN Complaint_Branch_Master as CBM ON CBM.Id = CU.BranchId
                                INNER JOIN Complaint_Department_Master AS CDM ON CDM.Id = CU.Dep_Id
                                WHERE CU.Id = @Id";

                // Use Dapper to query the single record
                var complaintDataTable = await _connectionService.SingleQueryReturn(query, Id);
                var row = complaintDataTable.Rows[0];
                UserModel model = new UserModel();

                model.Id = Convert.ToUInt16(row["Id"]);
                model.UserName = row["UserName"].ToString();
                model.Name = row["Name"].ToString();
                model.Email = row["Email"].ToString();
                model.BranchId = Convert.ToUInt16(row["BranchId"]);
                model.Branch = row["Branch"].ToString();
                model.Dep_Id = Convert.ToUInt16(row["Dep_Id"]);
                model.Department = row["Department"].ToString();
                model.CreatedDate = Convert.ToDateTime(row["CreatedDate"]);
                model.Active = Convert.ToBoolean(row["Active"]);
                model.Password = row["Password"].ToString();
                return (model);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void UpdateUser(IFormCollection collection)
        {
            try
            {
                var UserName = collection["UserNames"].ToString();
                var Name = collection["Names"].ToString();
                var Email = collection["Emails"].ToString();
                var BranchId = collection["BranchIds"].ToString();
                var Dep_Id = collection["Dep_Ids"].ToString();
                var id = collection["Id"].ToString();
                //var Password = collection["Password"].ToString();
                //string EncryptNewPassword = _passwordHelper.ComputeHmac(Password);
                string query = @"
                                UPDATE Complaint_User
                                SET 
                                    UserName = @uname,
                                    Name = @name,
                                    Email = @email,
                                    BranchId = @branchId,
                                    Dep_Id = @depId
                                WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Convert.ToInt64(id), DbType.Int64);
                parameters.Add("@uname", UserName, DbType.String);
                parameters.Add("@name", Name, DbType.String);
                parameters.Add("@email", Email, DbType.String);
                parameters.Add("@branchId", Convert.ToInt64(BranchId), DbType.Int64);
                parameters.Add("@depId", Convert.ToInt64(Dep_Id), DbType.Int64);

                _connectionService.ExecuteWithPara(query, parameters);
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ResetPassword(IFormCollection collection)
        {
            try
            {
                var id = collection["Id"].ToString();
                var Password = collection["Password"].ToString();
                string EncryptNewPassword = _passwordHelper.ComputeHmac(Password);
                string query = @"
                                UPDATE Complaint_User
                                SET 
                                    Password = @password,
                                    IsReset = @isReset
                                WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Convert.ToInt64(id), DbType.Int64);
                parameters.Add("@password", EncryptNewPassword, DbType.String);
                parameters.Add("@isReset", 0, DbType.Int32);
                _connectionService.ExecuteWithPara(query, parameters);
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void InactiveActive(int id, int status)
        {
            try
            {
                string query = @"
                                UPDATE Complaint_User
                                SET Active = @active
                                WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Convert.ToInt64(id), DbType.Int64);
                parameters.Add("@active", Convert.ToInt32(status), DbType.Int32);

                _connectionService.ExecuteWithPara(query, parameters);
                return;
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

        public void grantPermssion(IFormCollection collection)
        {
            try
            {
                var UserId = collection["Id"].ToString();
                var UserRoleId = collection["UserRoleId"].ToString();
                var userP = getPermissionList(Convert.ToInt32(UserId));
                var query = "INSERT INTO Complaint_User_Permission (UserId, UserRoleId, Active, CreatedDate) " +
                    "VALUES (@uId, @uRoleId, @active, @createdDate)";

                var parameters = new DynamicParameters();
                parameters.Add("uId", Convert.ToInt64(UserId), DbType.Int64);
                parameters.Add("uRoleId", Convert.ToInt64(UserRoleId), DbType.Int64);
                parameters.Add("createdDate", System.DateTime.Now, DbType.DateTime);
                parameters.Add("active", 1, DbType.Int32);

                _connectionService.ReturnWithPara(query, parameters);
                //Handle file upload
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserPermissionModel> getPermissionList(int Id)
        {
            try
            {
                string query = $@"
                                SELECT *
                                FROM Complaint_User_Permission 
                                WHERE UserId = {Id} AND Active=1";

                // Use Dapper to query the single record
                var complaintDataTable = _connectionService.Return(query);
                if (complaintDataTable.Rows.Count != 0)
                {
                    var row = complaintDataTable.Rows[0];
                    UserPermissionModel model = new UserPermissionModel();

                    model.Id = Convert.ToUInt16(row["Id"]);
                    model.UserId = Convert.ToUInt16(row["UserId"]);
                    model.UserRoleId = Convert.ToUInt16(row["UserRoleId"]); ;
                    model.CreatedDate = Convert.ToDateTime(row["CreatedDate"]);
                    model.Active = Convert.ToBoolean(row["Active"]);
                    return (model);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void deletePermission(IFormCollection collection)
        {
            try
            {
                var UserRoleId = collection["UserRoleId"].ToString();
                var UserId = Convert.ToInt32(collection["Id"].ToString());
                string query = @"
                                UPDATE Complaint_User_Permission
                                SET 
                                    Active = @active
                                WHERE UserId = @uId";

                var parameters = new DynamicParameters();
                parameters.Add("@uId", Convert.ToInt64(UserId), DbType.Int64);
                parameters.Add("@active", 0, DbType.Int32);
                _connectionService.ExecuteWithPara(query, parameters);
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void updatePermssion(IFormCollection collection)
        {
            try
            {
                var UserRoleId = collection["UserRoleId"].ToString();
                var UserId = Convert.ToInt32(collection["Id"].ToString());
                string query = @"
                                UPDATE Complaint_User_Permission
                                SET 
                                    UserRoleId = @uRoleId
                                WHERE UserId = @uId";

                var parameters = new DynamicParameters();
                parameters.Add("@uId", Convert.ToInt64(UserId), DbType.Int64);
                parameters.Add("@uRoleId", Convert.ToInt64(UserRoleId), DbType.Int64);

                _connectionService.ExecuteWithPara(query, parameters);
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UserModel checkUserName(string username)
        {
            try
            {
                string query = @$"
                                SELECT CU.Id AS Id, 
                                    CU.UserName AS UserName, 
                                    CU.Name AS Name, 
                                    CU.Email AS Email, 
                                    CBM.Id AS BranchId, 
                                    CBM.Branch AS Branch, 
                                    CDM.Id as Dep_Id, 
                                    CDM.Name AS Department , 
                                    CU.CreatedDate AS CreatedDate, 
                                    CU.IsReset AS IsReset,
                                    CU.Active AS Active,   
                                    CU.Password as Password
                                FROM Complaint_User AS CU 
                                INNER JOIN Complaint_Branch_Master as CBM ON CBM.Id = CU.BranchId
                                INNER JOIN Complaint_Department_Master AS CDM ON CDM.Id = CU.Dep_Id
                                WHERE CU.UserName = '" + username + "'";

                // Use Dapper to query the single record
                var complaintDataTable = _connectionService.Return(query);
                if (complaintDataTable.Rows.Count != 0)
                {
                    var row = complaintDataTable.Rows[0];
                    UserModel model = new UserModel();

                    model.Id = Convert.ToUInt16(row["Id"]);
                    model.UserName = row["UserName"].ToString();
                    model.Name = row["Name"].ToString();
                    model.Email = row["Email"].ToString();
                    model.BranchId = Convert.ToUInt16(row["BranchId"]);
                    model.Branch = row["Branch"].ToString();
                    model.Dep_Id = Convert.ToUInt16(row["Dep_Id"]);
                    model.Department = row["Department"].ToString();
                    model.CreatedDate = Convert.ToDateTime(row["CreatedDate"]);
                    model.Active = Convert.ToBoolean(row["Active"]);
                    model.Password = row["Password"].ToString();
                    return model;
                }
                return null;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}

