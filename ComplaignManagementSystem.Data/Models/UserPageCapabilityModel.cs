using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.Data.Models
{
    public class UserPageCapabilityModel
    {
        public int Id { get; set; }
        public int UserRoleId { get; set; }
        public string UserRole { get; set; }
        public int PageId { get; set; }
        public string Page { get; set; }
        public bool IsEdit { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
