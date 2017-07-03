using AutoMapper;
using Avengers.RepositoryModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSPRS.Avengers.DomainModels;
using PSPRS.Avengers.Repository.Contracts;
using PSPRS.Avengers.Repository.Contracts.Fakes;
using System.Net.Mail;

namespace PSPRS.Avengers.Core.Tests
{
    [TestClass()]
    public class EmailServiceTests
    {
        [TestInitialize()]
        public void init()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<EmailHistoryRM, EmailHistoryDM>();
            });
        }

        // TODO:  still need to test SendEmail method
        [TestMethod()]
        public void SendEmailTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void GenerateEmailHistoryEntryTest()
        {
            MailMessage message = new MailMessage
            {
                Sender= new MailAddress("sender@sender.com"),
                Subject = "subject",
                Body = "body",
                From = new MailAddress("sender@sender.com")
            };
            SmtpClient client = new SmtpClient
            {
                Host = "host",

            };
            EmailHistoryRM repoModel = new EmailHistoryRM
            {
                Sender = "sender",
                Recipient = "recipient@recipient.com",
                Subject = "subject",
                Body = message.Body,
                Host = client.Host,
                EmailTypeId = 1,
                CreatedUser = message.From.ToString(),
            };
            EmailHistoryDM domainModel = null;
            IEmailRepo repo = new StubIEmailRepo
            {
                GenerateEmailHistoryEntryEmailHistoryRM = passedRM =>
                {
                    passedRM = repoModel;
                    domainModel = Mapper.Map<EmailHistoryRM, EmailHistoryDM>(passedRM);
                }
            };
            EmailService service = new EmailService(repo);
            service.GenerateEmailHistoryEntry(message, repoModel.Recipient, client, 1);
            Assert.AreEqual(domainModel.CreatedUser, repoModel.CreatedUser);
            Assert.AreEqual(domainModel.Sender, repoModel.Sender);
            Assert.AreEqual(domainModel.Subject, repoModel.Subject);
            Assert.AreEqual(domainModel.Body, repoModel.Body);
            Assert.AreEqual(domainModel.Host, repoModel.Host);
            Assert.AreEqual(domainModel.EmailTypeId, repoModel.EmailTypeId);
            Assert.AreEqual(domainModel.Recipient, repoModel.Recipient);

        }
    }
}