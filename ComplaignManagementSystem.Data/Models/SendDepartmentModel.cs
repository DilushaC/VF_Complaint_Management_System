using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.Data.Models
{
    public class SendDepartmentModel
    {
        public int Id { get; set; }
        public int ComplaintMngProcess_Id { get; set; }
        public int Dep_Id { get; set; }
        public string DepartmentName { get; set; }
        public int EscalatiomMatrix { get; set; }
        public bool Active { get; set; }
        public bool Status { get; set; }
        public string Remark { get; set; }
        public string ForwardUser { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
