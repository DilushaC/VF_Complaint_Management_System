using ComplaignManagementSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.EmailHandler
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);
        //Task SendAsync2(EmailRequest<List<Complaint_ManageProcessModel>> request);
    }
}
