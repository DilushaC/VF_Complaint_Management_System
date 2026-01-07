using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.Data.Models
{
    public class EmailModel
    {
        public int Id { get; set; }
        public int ComProcessId { get; set; }
        public string EmailType { get; set; }
        public string EmailTemplateName { get; set; }
        public bool IsSent { get; set; }
        public bool Active { get; set; }
    }
}
