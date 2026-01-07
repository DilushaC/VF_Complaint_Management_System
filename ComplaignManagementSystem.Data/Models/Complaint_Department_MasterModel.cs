using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.Data.Models
{
    public class Complaint_Department_MasterModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string DepHeadName { get; set; }
        public string DepHeadEmail { get; set; }
        public string DepResName { get; set; }
        public string DepResEmail { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
