using ComplaignManagementSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.LoginHandler
{
    public interface ILoginService
    {
        Task<UserModel> ValidateUserAsync(string username, string password);
    }
}
