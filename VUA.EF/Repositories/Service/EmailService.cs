using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using VUA.Core.Models;

namespace VUA.EF.Repositories.Service
{
    public class EmailService : IEmailService
    {
        private const string templatePath = @"EmailTemplet/{0}.html";
        private readonly IConfiguration _configuration;
        private readonly SMTPConfigration _smtpConfigration;

        public async Task SendtestEmail(UserEmailOptions useremailOptions)
        {
            useremailOptions.Subject = "This is test email subject from VUA";
            useremailOptions.Body = GetEmailBody("EmailConfirm");
            await SendEmail(useremailOptions);
        }
        public async Task SendEmailForConfirmation(UserEmailOptions useremailOptions)
        {
            useremailOptions.Subject =UpdatePlaceHolders("Hello{{UserName}} This is test email subject from VUA" , useremailOptions.PlaceHolders!);
            useremailOptions.Body = UpdatePlaceHolders(GetEmailBody("EmailConfirm"),useremailOptions.PlaceHolders!);
            await SendEmail(useremailOptions);
        }
        public EmailService(IConfiguration configuration,
            SMTPConfigration smtpConfigration)
        {
            _configuration = configuration;
            _smtpConfigration = smtpConfigration;

        }


        private async Task SendEmail(UserEmailOptions userEmailOptions)
        {

            MailMessage mail = new MailMessage
            {
                Subject = userEmailOptions.Subject,
                Body = userEmailOptions.Body,
                From = new MailAddress(_smtpConfigration.SenderAddress!,_smtpConfigration.SenderDisplayName),
                IsBodyHtml =_smtpConfigration.IsBodyHTML
               
            };
            foreach (var toEmail in userEmailOptions.ToEmails!)
            {
                mail.To.Add(toEmail);
            }
            var smtpClient = new SmtpClient
            {
                Host =_smtpConfigration.host!,
                Port =_smtpConfigration.Port,
                EnableSsl = _smtpConfigration.EnableSSL,
                UseDefaultCredentials =_smtpConfigration.UserDefaultCredentials,
                Credentials =  new NetworkCredential(_smtpConfigration.UserName,_smtpConfigration.Password)

        };
            mail.BodyEncoding = Encoding.Default;
            try
            {
                await smtpClient.SendMailAsync(mail);
            }
            catch(Exception ex) 
            {
                string err= ex.ToString();
                string s = err;
            }
            
        }
        private string GetEmailBody(string templateName)
        {
            var body = File.ReadAllText(string.Format(templatePath, templateName));
            return body;
        }
        private string UpdatePlaceHolders(string text, List<KeyValuePair<string,string>> keyValuePairs)
        {
            if (!string.IsNullOrEmpty(text) && keyValuePairs!=null)
            {
                foreach (var placeholder in keyValuePairs)
                {
                    if (text.Contains(placeholder.Key))
                    {
                        text = text.Replace(placeholder.Key, placeholder.Value);
                    }
                }
            }
            return text;
        }

        Task IEmailService.SendtestEmail(UserEmailOptions useremailOptions)
        {
            throw new NotImplementedException();
        }

        Task IEmailService.SendEmailForConfirmation(UserEmailOptions useremailOptions)
        {
            throw new NotImplementedException();
        }

        Task IEmailService.SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }
    }
}
