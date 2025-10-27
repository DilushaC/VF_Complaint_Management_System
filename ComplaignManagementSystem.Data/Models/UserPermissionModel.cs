using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.Data.Models
{
    public class UserPermissionModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int UserRoleId { get; set; }
        public string Role { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
