using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.Data.Models
{
    public class ComplaintMaster
    {
        public int Id { get; set; }
        public string Refference { get; set; }
        public string Method { get; set; }
        public string Complaint { get; set; }
        public string CreatedUser { get; set; }
        public string Department { get; set; }
        public string Nature { get; set; }
        public string Branch { get; set; }
        public string ComBranch { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Cus_Email { get; set; }
        public string Cus_Name { get; set; }
        public string Cus_Nic { get; set; }
        public string Cus_Refference { get; set; }
        public string Cus_MobileNumber { get; set; }
        public bool IsCentralComment { get; set; }
        public string CentralComment { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
        public string AttachmentPath { get; set; }
    }
}
