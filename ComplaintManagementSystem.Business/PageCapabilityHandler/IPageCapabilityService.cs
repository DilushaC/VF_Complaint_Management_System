using ComplaignManagementSystem.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.PageCapabilityHandler
{
    public interface IPageCapabilityService
    {
        public List<PageCapabilityModel> getAllList();
        public Task<List<UserRoleModel>> getUserRoleList();
        public Task<List<PageModel>> getPageList();
        public void Create(IFormCollection collection);
        public bool CheckAvailability(int roleId, int pageId);
        public PageCapabilityModel getPageCapListId(int Id);
        public void Update(IFormCollection collection);
        public void Delete(int id);

    }
}
