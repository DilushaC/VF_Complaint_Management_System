using ComplaignManagementSystem.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.MethodHandler
{
    public interface IMethodService
    {
        public List<Complaint_Method_MasterModel> getAllList();
        public void CreateMethod(IFormCollection collection);
        public Complaint_Method_MasterModel getMethodListId(int Id);
        public void UpdateMethod(IFormCollection collection);
        public void DeleteMethod(int id);
    }
}
