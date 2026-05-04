using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.Data.Models
{
    public class PendingDetailsModel
    {
        public int Id { get; set; }
        public int ProcessId { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
    }
}
