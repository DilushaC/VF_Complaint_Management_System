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
        public void CreateAndSendComplaint(IFormCollection collection, IFormFile file);
        public void CreateComplaint(IFormCollection collection, IFormFile file);
        public void UpdateComplaint(IFormCollection collection, IFormFile file);
        public void UpdateSendComplaint(IFormCollection collection, IFormFile file);
        public Task<PaginationResultsModel<ComplaintMaster>> getComplaintList(int pageNumber, int pageSize, string searchString, string ComplaintMethod_Id = null, string priority = null);
        public Task<PaginationResultsModel<ComplaintMaster>> getDepComplaintList(int pageNumber, int pageSize, string searchString, string ComplaintMethod_Id = null, string priority = null);
        public Task<PaginationResultsModel<ComplaintMaster>> getCentralComplaintList(int pageNumber, int pageSize, string searchString, string ComplaintMethod_Id = null, string priority = null);
        public Task<ComplaintMaster> getComplainUsingId(int Id);
        public Complaint_ManageProcessModel getComplainProcessUsingId(int Id);
        public void UpdateForwardToCentral(int CompId, string remark);
        public void ComplainResolve(int Id, string Remark);
        public void UpdateForwardToDepartment(int CompId, int Department);        
        public Task<DashboardComplaintCountModel> GetDashboardComplaintCounts();
        
    }
}
