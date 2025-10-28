using ComplaignManagementSystem.Data.Context;
using ComplaignManagementSystem.Data.Models;
using ComplaintManagementSystem.Business.ConncetionHandler;
using ComplaintManagementSystem.Business.Helpers;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.LoginHandler
{
    public class LoginService : ILoginService
    {
        private readonly _ConnectionService _connectionService;
        private readonly PasswordHelper _passwordHelper;

        public LoginService(_ConnectionService connectionService, PasswordHelper passwordHelper)
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
                                              Active = row.Field<bool>("Active")
                                          })
                                          .ToList();

            var user = users.FirstOrDefault();
            if (user == null)
                return null;

            bool isValid = _passwordHelper.VerifyPassword(password, user.Password);
            return isValid ? user : null;
        }
    }
}
