using Avengers.RepositoryModels;
using PSPRS.Avengers.Core.Contracts;
using PSPRS.Avengers.DomainModels;
using PSPRS.Avengers.Repository.Contracts;
using System;
using System.Net;
using System.Net.Mail;


namespace PSPRS.Avengers.Core
{
    public class EmailService : IEmailService
    {
        private IEmailRepo emailRepo;

        public EmailService(IEmailRepo emailRepo)
        {
            this.emailRepo = emailRepo;
        }

        public void SendEmail(MailMessage message, string emailAddress, SmtpClient client)
        {
            if (client != null)
            {
                try
                {
                    client.Send(message);
                }
                catch(Exception e)
                {
                    throw new SmtpException(e.Message);
                }
            }
        }

        public MailMessage GenerateMailMessage(string from, string to, string subject, string body)
        {
            return new MailMessage(from, to, subject, body)
            {
                IsBodyHtml = true,
            };
        }

        public SmtpClient GenerateSmtpClient()
        {
            // using PSPRS host
            //return new SmtpClient
            //{
            //    Port = 25,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    Host = "exchange02.psprs.com",
            //    //Host = "10.5.100.100",
            //};


            //using GMail host
            return new SmtpClient
            {
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential("psprstester@gmail.com", "psprs123"),
                Host = "smtp.gmail.com",
                EnableSsl = true
            };
        }
        public void GenerateEmailHistoryEntry(MailMessage message, string emailAddress, SmtpClient client, int emailTypeId)
        {
            EmailHistoryRM emailHistory = new EmailHistoryRM
            {
                Sender = message.From.ToString(),
                Recipient = emailAddress,
                Subject = message.Subject,
                Body = message.Body,
                Host = client.Host,
                EmailTypeId = emailTypeId,
                CreatedUser = message.From.ToString(),
            };
            emailRepo.GenerateEmailHistoryEntry(emailHistory);
        }

        public string GeneratePasswordResetBody(string callbackUrl)
        {
            return string.Format("Your temporary password is: {0}<br /><a href = {1}>Click here to change your password</a>", callbackUrl);
        }

        public string GenerateEmailConfirmationBody(string body, string callbackUrl)
        {
            return string.Format(body, callbackUrl);
        }

        public string GenerateTemporaryPassword()
        {
            string password = string.Empty;
            string passwordChars = "abcdefghijklmnopqrstuvwxyz0123456789!%@&$ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random r = new Random();
            for (int i = 0; i <= 7; i++)
            {
                int next = r.Next(passwordChars.Length);
                password += passwordChars.Substring(next, 1);
            }
            return password;
        }
    }
}
