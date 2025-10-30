using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.Data.Models
{
    public class DashboardComplaintCountModel
    {
        public int TotalCount { get; set; }
        public int PendingCount { get; set; }
        public int ResolveCount { get; set; }
        public DateTime FromCreatedDate { get; set; }
        public DateTime ToCreatedDate { get; set; }


        public List<ComplaintMethodCountModel> ComplaintMethodCounts { get; set; }

    }


    public class ComplaintMethodCountModel
    {
        public string Method { get; set; }
        public int ComplaintCount { get; set; }
    }
}
