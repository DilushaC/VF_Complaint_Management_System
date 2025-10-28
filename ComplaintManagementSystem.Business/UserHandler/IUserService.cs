using ComplaignManagementSystem.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.LoginHandler
{
    public interface IUserService
    {
        Task ResetPassword(string userId, string saltKey, string NewPassword);
        Task<UserModel> ValidateUserAsync(string username, string password);
        UserPermissionModel getAccessPerimissions(UserModel user);
        List<UserPageCapabilityModel> getAccessPages(UserModel user, UserPermissionModel uPermission);
    }
}
