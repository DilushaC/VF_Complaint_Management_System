using ComplaignManagementSystem.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.NatureHandler
{
    public interface INatureService
    {
        public List<NatureModel> getAllList();
        public Task<List<Complaint_Department_MasterModel>> getDepList();
        public void CreateNature(IFormCollection collection);
        public NatureModel getNatureListId(int Id);
        public void UpdateNature(IFormCollection collection);
        public void DeleteNature(int id);
    }
}
