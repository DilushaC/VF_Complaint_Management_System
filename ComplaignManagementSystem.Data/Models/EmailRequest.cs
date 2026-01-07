using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.Data.Models
{
    public class EmailRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string TemplateName { get; set; }
        public object Model { get; set; }
        public List<string> ccEmailsModel { get; set; } = new();
        //public T ModelList { get; set; }

    }
}
