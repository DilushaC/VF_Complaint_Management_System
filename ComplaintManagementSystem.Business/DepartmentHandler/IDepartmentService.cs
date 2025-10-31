using ComplaignManagementSystem.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.DepartmentHandler
{
    public interface IDepartmentService
    {
        public List<DepartmentModel> getAllList();
        public void CreateDepartment(IFormCollection collection);
        public DepartmentModel getDepListId(int Id);
        public void UpdateDepartment(IFormCollection collection);
        public void DeleteDepartment(int id);
    }
}
