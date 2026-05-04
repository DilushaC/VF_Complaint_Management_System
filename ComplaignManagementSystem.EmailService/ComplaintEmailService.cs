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

        public async Task SendApprovalEmail()
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

                itemA.ApproverName = resPerson.DepResName;
                toEmail = resPerson.DepResEmail + "," + resBPerson.BranchEmail;
                EmailRequest request = new EmailRequest();

                if (diffDays < 2)
                {
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
                    var Ccemails = ccEmails.Result
                                .Where(a => a.Status == 1)
                                .Select(a => a.Email)
                                .Where(e => !string.IsNullOrWhiteSpace(e))
                                .ToList();

                    if (!string.Equals(resPerson.DepHeadEmail, resPerson.DepResEmail, StringComparison.OrdinalIgnoreCase))
                    {
                        Ccemails.Add(resPerson.DepHeadEmail);
                    }

                    request = new EmailRequest
                    {
                        To = toEmail,
                        //To = toEmail,
                        Subject = "COMPLAINT MANAGEMENT | Pending Approval Notification",
                        TemplateName = "ApprovalsTemplate",
                        Model = itemA,
                        ccEmailsModel = Ccemails.Distinct(StringComparer.OrdinalIgnoreCase).ToList()
                    };
                }
                //request.ccEmailsModel.AddRange(resBPerson.BranchEmail);
                await _emailService.SendAsync(request);
                File.AppendAllText(
                                    "email-log.txt",
                                    $"Email job ran at {DateTime.Now}{Environment.NewLine}");

            }
        }

        public async Task SendCentrailEmail()
        {
            try
            {
                Console.WriteLine("Enter Email");
                var ComplaintList = _comManageProcess.getCreatedCentralComplainLists().Result.ToList();
                Console.WriteLine("success getting list");

                var ccEmails = _comManageProcess.getCcEmails();
                //var DepIdDistint = ComplaintList.Select(a => a.Dep_Id).Distinct().ToList();

                Console.WriteLine("before foreach");

                foreach (var itemA in ComplaintList)
                {
                    string toEmail;
                    DateTime systemDate;
                    int diffDays;
                    var resPerson = _comManageProcess.getCentralResPersons().Result.ToList();
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
                    toEmail = string.Join(",", ccEmails.Result.Where(x => !string.IsNullOrWhiteSpace(x.Email)).Select(x => x.Email.Trim()));
                    EmailRequest request = new EmailRequest();
                    request = new EmailRequest
                    {
                        To = toEmail,
                        //To = toEmail,
                        Subject = "COMPLAINT MANAGEMENT | Pending Approval Notification Central",
                        TemplateName = "CentralTemplate",
                        Model = itemA,
                        //ccEmailsModel = ccEmails.Result.Where(a => a.Status == 1)
                        //                    .Select(a => a.Email)
                        //                    .Distinct(StringComparer.OrdinalIgnoreCase)
                        //                    .ToList()
                    };
                    //request.ccEmailsModel.AddRange(resBPerson.BranchEmail);
                    Console.WriteLine("Success send data to email service.");
                    await _emailService.SendAsync(request);
                    Console.WriteLine("Successfully send.");
                    File.AppendAllText(
                                        "email-log.txt",
                                        $"Email job ran at {DateTime.Now}{Environment.NewLine}");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
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
