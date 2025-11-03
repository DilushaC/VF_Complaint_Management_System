using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.Data.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }  // stored hashed password
        public string SaltKey { get; set; }   // added field in DB
        public string Name { get; set; }
        public string Email { get; set; }
        public int? BranchId { get; set; }
        public string Branch { get; set; }
        public int? Dep_Id { get; set; }
        public string Department { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
        public bool IsReset { get; set; }
    }
}
