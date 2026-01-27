using ComplaignManagementSystem.Data.Models;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.EmailHandler
{
    public class EmailService : IEmailService
    {
        private readonly IEmailTemplateRenderer _renderer;
        private readonly IConfiguration _config;

        public EmailService(IEmailTemplateRenderer renderer, IConfiguration config)
        {
            _renderer = renderer;
            _config = config;
        }

        public async Task SendAsync(EmailRequest request)
        {
            try
            {
                var toEmails = request.To.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim());
                var body = await _renderer.RenderAsync(request.TemplateName, request.Model);
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(_config["Email:Smtp:Username"], _config["Email:Smtp:From"]));

                //foreach (var email in toEmails)
                //{
                //    emailMessage.To.Add(new MailboxAddress(email, email));
                //}

                emailMessage.To.Add(new MailboxAddress("kasunperera@vallibelfinance.com", "kasunperera@vallibelfinance.com"));
                //emailMessage.To.Add(new MailboxAddress(request.To, request.To));

                var ccEmails = request.ccEmailsModel;

                foreach (var cc in ccEmails)
                {
                    emailMessage.Cc.Add(new MailboxAddress(cc, cc));
                }

                emailMessage.Subject = request.Subject;
                emailMessage.Body = new TextPart("html") { Text = body };

                using (var client = new SmtpClient())
                {
                    // Port 465 requires SSL
                    client.Connect(_config["Email:Smtp:Host"], int.Parse(_config["Email:Smtp:Port"]), true);
                    // Authenticate
                    client.Authenticate(_config["Email:Smtp:From"], _config["Email:Smtp:Password"]);
                    // Send email
                    client.Send(emailMessage);
                    client.Disconnect(true);
                }    
            }
            catch (Exception ex)
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(_config["Email:Smtp:Username"], _config["Email:Smtp:From"]));
                emailMessage.To.Add(new MailboxAddress("kasunperera@vallibelfinance.com", "kasunperera@vallibelfinance.com"));
                emailMessage.Subject = "request.Subject : Error Email Complaints Management";
                emailMessage.Body = new TextPart("plain") { Text = ex.Message };
                using (var client = new SmtpClient())
                {
                    // Port 465 requires SSL
                    client.Connect(_config["Email:Smtp:Host"], int.Parse(_config["Email:Smtp:Port"]), true);
                    // Authenticate
                    client.Authenticate(_config["Email:Smtp:From"], _config["Email:Smtp:Password"]);
                    // Send email
                    client.Send(emailMessage);
                    client.Disconnect(true);
                }
            }
        }

    }
}
