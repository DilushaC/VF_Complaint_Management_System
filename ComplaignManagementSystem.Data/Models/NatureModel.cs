using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.Data.Models
{
    public class NatureModel
    {
        public int Id { get; set; }
        public int Dep_Id { get; set; }
        public string Dep_Name { get; set; }
        public string Nature { get; set; }
        public string NatureSinhala { get; set; }
        public string Code { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
