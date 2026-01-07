using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.EmailHandler
{
    public interface IEmailTemplateRenderer
    {
        Task<string> RenderAsync(string templateName, object model);
    }
}
