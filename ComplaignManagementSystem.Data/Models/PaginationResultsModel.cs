using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.Data.Models
{
    public class PaginationResultsModel<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }

    }
}
