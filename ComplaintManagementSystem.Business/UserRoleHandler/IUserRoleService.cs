using ComplaignManagementSystem.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.UserRoleHandler
{
    public interface IUserRoleService
    {
        public List<UserRoleModel> getAllList();
        public void Create(IFormCollection collection);
        public UserRoleModel getListId(int Id);
        public void Update(IFormCollection collection);
        public void Delete(int id);
    }
}
