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
        DepartmentModel GetDepartmentDetails(int DepId);
        UserPermissionModel getAccessPerimissions(UserModel user);
        List<UserPageCapabilityModel> getAccessPages(UserModel user, UserPermissionModel uPermission);
        public List<UserModel> getAllList();
        public Task<List<Complaint_Department_MasterModel>> getDepList();
        public Task<List<BranchModel>> getBranchList();
        public void CreateUser(IFormCollection collection);
        public Task<UserModel> getUserDetailId(int Id);
        public UserModel checkUserName(string username);
        public void UpdateUser(IFormCollection collection);
        public void ResetPassword(IFormCollection collection);
        public void InactiveActive(int id, int status);
        public Task<List<UserRoleModel>> getUserRoleList();
        public void grantPermssion(IFormCollection collection);
        public void deletePermission(IFormCollection collection);
        public Task<UserPermissionModel> getPermissionList(int Id);
        public void updatePermssion(IFormCollection collection);
    }
}
