using ComplaignManagementSystem.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComplaintManagementSystem.Business.ConncetionHandler._ConnectionService;

namespace ComplaintManagementSystem.Business.ComplaintManageProcessHandler
{
    public interface IComplaintManageProcessService
    {
        public Task<List<Complaint_Method_MasterModel>> getMethodList();
        public Task<List<Complaint_Department_MasterModel>> getDepList();
        public List<Complaint_Nature_MasterModel> GetNaturesByDepartment(int DepId);
        public void CreateComplaint(IFormCollection collection, IFormFile file);
        public Task<PaginationResultsModel<Complaint_ManageProcessModel>> getComplaintList(int pageNumber, int pageSize, string searchString);



    }
}
