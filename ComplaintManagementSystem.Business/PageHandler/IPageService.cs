using ComplaignManagementSystem.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.PageHandler
{
    public interface IPageService
    {
        public List<PageModel> getAllList();
        public void Create(IFormCollection collection);
        public PageModel getListId(int Id);
        public void Update(IFormCollection collection);
        public void Delete(int id);
    }
}
