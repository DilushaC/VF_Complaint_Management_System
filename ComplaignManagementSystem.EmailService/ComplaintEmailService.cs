using ComplaignManagementSystem.Data.Models;
using ComplaintManagementSystem.Business.ComplaintManageProcessHandler;
using ComplaintManagementSystem.Business.EmailHandler;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Packaging;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.EmailService
{
    public class ComplaintEmailService
    {
        private readonly IEmailService _emailService;
        private readonly IComplaintManageProcessService _comManageProcess;

        public ComplaintEmailService(IEmailService emailService, IComplaintManageProcessService complaintManage)
        {
            _emailService = emailService;
            _comManageProcess = complaintManage;
        }

        public async Task SendEmail()
        {
            var ComplaintList = _comManageProcess.getCreatedComplainLists().Result.ToList();
            //var DepIdDistint = ComplaintList.Select(a => a.Dep_Id).Distinct().ToList();
            var ccEmails = _comManageProcess.getCcEmails();
            foreach (var itemA in ComplaintList)
            {
                string toEmail;
                DateTime systemDate;
                int diffDays;
                var resPerson = _comManageProcess.getDepResPerson(itemA.Dep_Id);
                var resBPerson = _comManageProcess.getBranchResPerson(itemA.Branch_Id);
                switch (itemA.Priority)
                {
                    case "High":
                        systemDate = DateTime.Now.AddDays(-3).Date;
                        diffDays = (itemA.CreatedDate.Date.AddDays(3) - DateTime.Now.Date).Days;
                        break;
                    case "Medium":
                        systemDate = DateTime.Now.AddDays(-5).Date;
                        diffDays = (itemA.CreatedDate.Date.AddDays(5) - DateTime.Now.Date).Days;
                        break;
                    default:
                        systemDate = DateTime.Now.AddDays(-7).Date;
                        diffDays = (itemA.CreatedDate.Date.AddDays(7) - DateTime.Now.Date).Days;
                        break;
                }
                itemA.DiffDays = diffDays;
                //if (itemA.CreatedDate.Date < systemDate)
                //{
                //    itemA.ApproverName = resPerson.DepResName;
                //    //itemA.ApproverName = resPerson.DepHeadName;
                //    toEmail = resPerson.DepHeadEmail + resBPerson.BranchEmail;
                //}
                //else
                //{
                //    itemA.ApproverName = resPerson.DepResName;
                //    toEmail = resPerson.DepResEmail;
                //}
                itemA.ApproverName = resPerson.DepResName;
                toEmail = resPerson.DepResEmail + "," + resBPerson.BranchEmail;
                EmailRequest request = new EmailRequest();
                if (diffDays < 2)
                {
                    //var ccEmailsModel = ccEmails.Result.Select(a => a.Email).ToList();
                    //if (!string.IsNullOrWhiteSpace(resBPerson.BranchEmail) &&
                    //    !ccEmailsModel.Any(e =>
                    //        e.Equals(resBPerson.BranchEmail, StringComparison.OrdinalIgnoreCase)))
                    //{
                    //    ccEmailsModel.Add(resBPerson.BranchEmail);
                    //}
                    request = new EmailRequest
                    {
                        To = toEmail,
                        //To = toEmail,
                        Subject = "COMPLAINT MANAGEMENT | Pending Approval Notification",
                        TemplateName = "ApprovalsTemplate",
                        Model = itemA,
                        ccEmailsModel = ccEmails.Result
                                        .Select(a => a.Email)
                                        .Append(resPerson.DepHeadEmail)
                                        .Where(e => !string.IsNullOrWhiteSpace(e))
                                        .Distinct(StringComparer.OrdinalIgnoreCase)
                                        .ToList()
                    };
                }
                else
                {
                    request = new EmailRequest
                    {
                        To = toEmail,
                        //To = toEmail,
                        Subject = "COMPLAINT MANAGEMENT | Pending Approval Notification",
                        TemplateName = "ApprovalsTemplate",
                        Model = itemA,
                        ccEmailsModel = ccEmails.Result.Where(a => a.Status == 1)
                                        .Select(a => a.Email)
                                        .Append(resPerson.DepHeadEmail)
                                        .Where(e => !string.IsNullOrWhiteSpace(e))
                                        .Distinct(StringComparer.OrdinalIgnoreCase)
                                        .ToList()
                    };
                }
                //request.ccEmailsModel.AddRange(resBPerson.BranchEmail);
                await _emailService.SendAsync(request);
                File.AppendAllText(
                                    "email-log.txt",
                                    $"Email job ran at {DateTime.Now}{Environment.NewLine}");

            }
        }

        public async Task SendEmail2()
        {
            var emailLists = _comManageProcess.getEmails().Result.ToList();
            //var DepIdDistint = ComplaintList.Select(a => a.Dep_Id).Distinct().ToList();
            var ccEmails = _comManageProcess.getCcEmails();

            foreach (var itemA in emailLists)
            {
                string toEmail;
                EmailRequest request = new EmailRequest();
                var ComplaintList = _comManageProcess.getCreatedComplainListsId(itemA.ComProcessId).Result;
                var resPerson = _comManageProcess.getDepResPerson(ComplaintList.Dep_Id);
                switch (itemA.EmailType)
                {
                    case "Resolved":
                        toEmail = resPerson.DepResEmail + "," + resPerson.DepHeadEmail + "," + ComplaintList.CreatedUserEmail + "," + ComplaintList.ResolvedUserEmail;
                        break;
                    default:
                        toEmail = "";
                        break;
                }

                request.To = toEmail;
                //request.To = toEmail,
                request.Subject = "COMPLAINT MANAGEMENT | Resolved Notification";
                request.TemplateName = itemA.EmailTemplateName;
                request.Model = ComplaintList;
                request.ccEmailsModel = ccEmails.Result.Select(a => a.Email).ToList();
                await _emailService.SendAsync(request);
                await _comManageProcess.deleteEmail(itemA.Id);

                File.AppendAllText(
                                    "email-log.txt",
                                    $"Email job ran at {DateTime.Now}{Environment.NewLine}");

            }
        }
    }
}
