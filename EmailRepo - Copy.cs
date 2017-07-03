using Avengers.RepositoryModels;
using PSPRS.Avengers.Repository.Contracts;
using System;
using System.Net.Mail;

namespace PSPRS.Avengers.Repository
{
    public class EmailRepo : SqlServerBase, IEmailRepo
    {
        public EmailRepo(string connectionString) : base(connectionString)
        {
                
        }

        public void GenerateEmailHistoryEntry(EmailHistoryRM emailHistoryEntry)
        {
            int result;
            using (this)
            {
                string storedProc = "email_history_insert";
                result = ExecuteNonQuery(storedProc, emailHistoryEntry);
            }
            if (result < 1)
            {
                throw new Exception("Failed to insert record");
            }
        }
    }
}
