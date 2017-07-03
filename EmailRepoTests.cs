using Avengers.RepositoryModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Net.Mail;

namespace PSPRS.Avengers.Repository.Tests
{
    [TestClass()]
    public class EmailRepoTests : BaseRepoTest
    {
        private MailMessage message;
        private SmtpClient client;
        private string emailAddress;

        [TestInitialize()]
        public void Init()
        {
            message = new MailMessage
            {
                Sender = new MailAddress("test@sender.com"),
                Subject = "test",
                Body = "test",
                From = new MailAddress("test@sender.com")
            };
            client = new SmtpClient
            {
                Host = "test",
            };
            emailAddress = "email@email.com";
        }

        [TestCleanup()]
        public void Cleanup()
        {
            string sql = string.Format("delete from email_history where email_id in(select top 1 email_id from email_history where created_user = '{0}')", 
                message.From.ToString());
            using(SqlConnection cn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        [TestMethod()]
        public void GenerateEmailHistoryEntryTest()
        {
            EmailRepo repo = new EmailRepo(connectionString);
            MailMessage message = this.message;
            SmtpClient client = this.client;
            EmailHistoryRM repoModel = new EmailHistoryRM
            {
                Sender = message.Sender.ToString(),
                Recipient = emailAddress,
                Subject = message.Subject,
                Body = message.Body,
                Host = client.Host,
                EmailTypeId = 1,
                CreatedUser = message.From.ToString(),
            };
            repo.GenerateEmailHistoryEntry(repoModel);
        }
    }
}