using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.Data.Models
{
    public class Complaint_ManageProcessModel
    {
        public int Id { get; set; }
        public int ComplaintMethod_Id { get; set; }
        public string ComplaintMethod { get; set; }
        public string Refference { get; set; }
        public string Complaint { get; set; }
        public string Cus_Email { get; set; }
        public string Cus_Name { get; set; }
        public string Cus_Nic { get; set; }
        public string Cus_Refference { get; set; }
        public string Cus_MobileNumber { get; set; }
        public int Dep_Id { get; set; }
        public string Dep { get; set; }
        public int Nature_Id { get; set; }
        public string Nature { get; set; }
        public int Branch_Id { get; set; }
        public string Branch { get; set; }
        public string ComBranch { get; set; }
        public string Priority { get; set; }
        public bool IsSentCentral { get; set; }
        public DateTime? IsSentCentralDateTime { get; set; }
        public bool IsSentDep { get; set; }
        public DateTime? IsSentDepDateTime { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string? ResolvedRemark { get; set; }
        public bool? IsResolved { get; set; }
        public DateTime? ResolvedDateTime { get; set; }
        public DateTime? CusNotifiedDate { get; set; }
        public string? ResolvedUser { get; set; }
        public string? ResolvedUserEmail { get; set; }
        public DateTime EditedDateTime { get; set; }
        public bool IsCentralComment { get; set; }
        public bool Active { get; set; }
        public DateTime DeletedDate { get; set; }
        public string CreatedBranch { get; set; }
        public string DeletedUser { get; set; }
        public string CreatedUser { get; set; }
        public string CreatedUserEmail { get; set; }
        public DateTime CreatedDate { get; set; }
        public IFormFile File { get; set; }
        public string AttachmentPath { get; set; }
        public string CAttachmentPath { get; set; }
        public string downloadUrl { get; set; }
        public bool? IsCusNotified { get; set; }
        public string CusNotification { get; set; }
        public string CusNotificationRemark { get; set; }
        public string CentralComment { get; set; }


        //------------
        public string ForwordUser { get; set; }
        public int MatrixOrder { get; set; }
        public string DepartmentName { get; set; }
        public string Remark { get; set; }
        public string ApproverName { get; set; }
        public int DiffDays { get; set; }


    }
}
