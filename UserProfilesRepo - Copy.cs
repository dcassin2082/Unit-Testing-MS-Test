using PSPRS.Avengers.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avengers.RepositoryModels;
using Avengers.RepositoryModels.Identity;

namespace PSPRS.Avengers.Repository
{
    public class UserProfilesRepo : SqlServerBase, IUserProfilesRepo
    {
        public UserProfilesRepo(string connectionString) : base(connectionString)
        {
        }

        public List<UserNameRM> GetUsers()
        {
             using (this)
            {
                string storedProc = "user_profiles_get_users";
                return ExecuteReader<UserNameRM>(storedProc);
            }
        }

        public List<UserEmployerRM> GetEmployers()
        {
            using (this)
            {
                string storedProc = "employers_get_all";
                return ExecuteReader<UserEmployerRM>(storedProc);
            }
        }

        
        public void AddUserToEmployer(UserEmployerQM queryModel)
        {
            int result;
            using (this)
            {
                string storedProc = "user_profiles_add_user_to_employer";
                result = ExecuteNonQuery(storedProc, queryModel);
            }
            if (result < 1)
            {
                throw new Exception("Failed to insert record");
            }
        }

        public void RemoveUserFromEmployer(UserEmployerQM queryModel)
        {
            int result;
            using (this)
            {
                string storedProc = "user_profiles_remove_user_from_employer";
                result = ExecuteNonQuery(storedProc, queryModel);
            }
            if (result < 1)
            {
                throw new Exception("Failed to delete record");
            }
        }

        public List<UserEmployerRM> GetEmployersByUserID(EmployerUserIDQM userID)
        {
            using (this)
            {
                string storedProc = "employers_get_employers_by_user_id";
                return ExecuteReader<UserEmployerRM, EmployerUserIDQM>(storedProc, userID);
            }
        }
    }
}
