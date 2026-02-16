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
        public Task<List<BranchModel>> getBranchList();
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
        public Task<Complaint_ManageProcessModel> getComplainMasterUsingId(int Id);
        public Complaint_ManageProcessModel getComplainProcessUsingId(int Id);
        public void UpdateForwardToCentral(int CompId, string remark);
        public void ComplainResolve(int Id, string Remark);
        public void ComplainCentralUpdate(int Id, string CentralComment);
        public void UpdateForwardToDepartment(int CompId, int Department, string remark);        
        public Task<DashboardComplaintCountModel> GetDashboardComplaintCounts();
        public Task<List<ComplaintMaster>> getComplainNumberList();
        public Task<List<Complaint_ManageProcessModel>> GetComplaintHistoryDetails(int Id);
        public Task<List<CustomerNotificationMasterModel>> getNotificationList();
        public void UpdateCustomerInformDetails(int ComplainNo, int NotifiID, string Complaint, bool isNotified, IFormFile file);
        public Task<List<ComplaintMaster>> getCusInfoCompNoList();
        public Task<List<ComplaintMaster>> getCusNotifiedCompNoList();
        public void DeleteComplaint(int id);
        public Task<List<Complaint_ManageProcessModel>> getCreatedComplainLists();
        public Task<List<Complaint_ManageProcessModel>> getCreatedCentralComplainLists();
        public Task<List<UserModel>> getCentralResPersons();
        public Task<Complaint_ManageProcessModel> getCreatedComplainListsId(int id);
        public Task<List<EmailModel>> getEmails();
        public Complaint_Department_MasterModel getDepResPerson(int depId);
        public BranchModel getBranchResPerson(int branchId);
        public Task<List<EmailRecipientsModel>> getCcEmails();
        public Task deleteEmail(int EmailId);
    }
}
