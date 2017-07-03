using PSPRS.Avengers.Repository.Contracts.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSPRS.Avengers.DomainModels;
using Avengers.RepositoryModels.Identity;
using Avengers.RepositoryModels;

namespace PSPRS.Avengers.Repository.Identity
{
    public class IdentityUserRepo : SqlServerBase, IIdentityUserRepo
    {
        public IdentityUserRepo(string connectionString) : base(connectionString)
        {
        }

        #region Gets
        public string GetEmailAsync(UserIDQM queryModel)
        {
            using (this)
            {
                string storedProc = "identity_users_get_email_address";
                return ExecuteReader<IdentityUserRM, UserIDQM>(storedProc, queryModel).Select(e => e.EmailAddress).FirstOrDefault();
            }
        }

        public bool GetEmailConfirmed(UserIDQM queryModel)
        {
            using (this)
            {
                string storedProc = "identity_users_get_email_confirmed";
                return ExecuteReader<IdentityUserRM, UserIDQM>(storedProc, queryModel).Count > 0;
            }
        }

        public IdentityUserRM GetUserByEmailAddress(GetUserEmailAddressQM queryModel)
        {
            using (this)
            {
                string storedProc = "identity_users_get_user_by_email_address";
                return ExecuteReader<IdentityUserRM, GetUserEmailAddressQM>(storedProc, queryModel).FirstOrDefault();
            }
        }

        public IdentityUserRM GetUserById(UserIDQM queryModel)
        {
            using (this)
            {
                string storedProc = "identity_users_get_user_by_user_id";
                return ExecuteReader<IdentityUserRM, UserIDQM>(storedProc, queryModel).FirstOrDefault();
            }
        }

        public IdentityUserRM GetUserByUserName(GetUserUserNameQM queryModel)
        {
            using (this)
            {
                string storedProc = "identity_users_get_user_by_username";
                return ExecuteReader<IdentityUserRM, GetUserUserNameQM>(storedProc, queryModel).FirstOrDefault();
            }
        }

        public IList<string> GetUserRoles(UserIDQM queryModel)
        {
            using (this)
            {
                string storedProc = "identity_get_user_roles";
                return ExecuteReader<IdentityRoleRM, UserIDQM>(storedProc, queryModel).Select(n => n.Name).ToList();
            }
        }

        public List<IdentityUserRM> GetUsers()
        {
            using (this)
            {
                string storedProc = "identity_users_get_users";
                return ExecuteReader<IdentityUserRM>(storedProc);
            }
        }

        public bool IsUserInRole(UserInRoleQM queryModel)
        {
            using (this)
            {
                string storedProc = "identity_user_roles_user_in_role";
                return ExecuteReader<IdentityUserRM, UserInRoleQM>(storedProc, queryModel).Count > 0;
            }
        }

        public void RemoveFromRole(IdentityUserRM user, UserInRoleQM queryModel)
        {
            int result;
            using (this)
            {
                string storedProc = "identity_user_roles_remove_user";
                result = ExecuteNonQuery(storedProc, queryModel);
            }
            if (result < 1)
            {
                throw new Exception("Failed to remove record");
            }
        }

        public bool UserHasPassword(UserIDQM queryModel)
        {
            using (this)
            {
                string storedProc = "identity_user_has_password";
                return ExecuteReader<IdentityUserRM, UserIDQM>(storedProc, queryModel).Count > 0;
            }
        }
        #endregion

        #region Saves
        public void AddUserToRole(IdentityUserDM user, string roleName)
        {
            int result;
            using (this)
            {
                string storedProc = "identity_user_roles_add_to_role";
                result = ExecuteNonQuery(storedProc, new UserRolesQM { UserID = user.Id, Name = roleName });
            }
            if (result < 1)
            {
                throw new Exception("Failed to insert record");
            }
        }

        public void CreateUser(IdentityUserRM user)
        {
            int result;
            IdentityCreateUserQM qm = new IdentityCreateUserQM
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                AccessFailedCount=user.AccessFailedCount,
                EmailAddress = user.EmailAddress,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEndDateUtc = user.LockoutEndDateUtc,
                PasswordHash = user.PasswordHash,
                SecurityStamp = user.SecurityStamp,
                UserName = user.UserName
            };
            using (this)
            {
                string storedProc = "identity_users_create_user";
                result = ExecuteNonQuery(storedProc, qm);
            }
            if (result < 1)
            {
                throw new Exception("Failed to insert record");
            }
        }

        public void UpdateUser(IdentityUserRM user)
        {
            int result;
            using (this)
            {
                string storedProc = "identity_users_update_user";
                result = ExecuteNonQuery(storedProc, user);
            }
            if (result < 1)
            {
                throw new Exception("Failed to update record");
            }
        }

        public void DeleteUser(UserIDQM queryModel)
        {
            int result;
            using (this)
            {
                string storedProc = "identity_users_delete_user";
                result = ExecuteNonQuery(storedProc, queryModel);
            }
            if (result < 1)
            {
                throw new Exception("Failed to delete record");
            }
        }
        #endregion
    }
}
