using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.Data.Models
{
    public class CustomerNotificationMasterModel
    {
        public int Id { get; set; }
        public string Notification { get; set; }
        public string Code { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
