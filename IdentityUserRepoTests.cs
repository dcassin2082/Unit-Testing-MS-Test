using Avengers.RepositoryModels.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSPRS.Avengers.DomainModels;
using PSPRS.Avengers.Repository.Tests;
using System;
using System.Data;
using System.Data.SqlClient;

namespace PSPRS.Avengers.Repository.Identity.Tests
{
    [TestClass()]
    public class IdentityUserRepoTests : BaseRepoTest
    {
        private static int _userId;
        private static string _emailAddress;
        private static string _username;
        private static string _roleName;
        private static int _roleId;
        private static int _addRoleId;
        private static string _addRoleName;

        [TestInitialize()]
        public void Init()
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                string sql = "insert into identity_users(username, email_address, password_hash, email_confirmed, lockout_enabled, access_failed_count, lockout_end_dt_utc)"
                    + " values('Tester', 'email@email.com', 'password', 1, 0, 0, getdate())";
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                string sql = "select * from identity_users where username = 'Tester'";
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    _userId = GetFieldValue<int>(dt.Rows[0], "user_id");
                    _emailAddress = GetFieldValue<string>(dt.Rows[0], "email_address");
                    _username = GetFieldValue<string>(dt.Rows[0], "username");
                }
            }
            // create a role
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                string sql = "insert into identity_roles(name) values('Tester')";
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                    sql = "insert into identity_roles(name) values('Add to role')";
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                string sql = "select * from identity_roles where name = 'Tester'";
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    _roleId = GetFieldValue<int>(dt.Rows[0], "role_id");
                    _roleName = GetFieldValue<string>(dt.Rows[0], "name");
                }
            }
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                string sql = "select * from identity_roles where name = 'Add to role'";
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    _addRoleId = GetFieldValue<int>(dt.Rows[0], "role_id");
                    _addRoleName = GetFieldValue<string>(dt.Rows[0], "name");
                }
            }
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                string sql = string.Format("insert into identity_user_roles(user_id, role_id) values({0}, {1})", _userId, _roleId);
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        static T GetFieldValue<T>(DataRow dr, string fieldName)
        {
            var value = dr[fieldName];
            if (value == null || value == DBNull.Value)
                return default(T);
            else
                value = Convert.ChangeType(value, typeof(T));
            return (T)value;
        }
        [TestCleanup]
        public void Cleanup()
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                string sql = string.Format("delete from identity_user_roles where user_id = {0}", _userId);
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                    sql = "delete from identity_users";
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    sql = "delete from identity_roles";
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                string sql = "delete from identity_users where username = 'create@user.com' or username = 'email@email.com'";
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                string sql = string.Format("delete from identity_roles where name = '{0}'", _addRoleName);
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        [TestMethod()]
        [Ignore]
        public void GetUserByEmailAddressTest()
        {
            IdentityUserRepo repo = new IdentityUserRepo(connectionString);
            GetUserEmailAddressQM qm = new GetUserEmailAddressQM { EmailAddress = _emailAddress };
            var result = repo.GetUserByEmailAddress(qm);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        [Ignore]
        public void GetUserByIdTest()
        {
            IdentityUserRepo repo = new IdentityUserRepo(connectionString);
            UserIDQM qm = new UserIDQM { UserID = _userId };
            var result = repo.GetUserById(qm);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        [Ignore]
        public void GetUserByUserNameTest()
        {
            IdentityUserRepo repo = new IdentityUserRepo(connectionString);
            GetUserUserNameQM qm = new GetUserUserNameQM { UserName = _username };
            var result = repo.GetUserByUserName(qm);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        [Ignore]
        public void GetUserRolesTest()
        {
            IdentityUserRepo repo = new IdentityUserRepo(connectionString);
            UserIDQM qm = new UserIDQM { UserID = _userId };
            var result = repo.GetUserRoles(qm);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        [Ignore]
        public void GetUsersTest()
        {
            IdentityUserRepo repo = new IdentityUserRepo(connectionString);
            var result = repo.GetUsers();
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        [Ignore]
        public void IsUserInRoleTest_True()
        {
            IdentityUserRepo repo = new IdentityUserRepo(connectionString);
            UserInRoleQM qm = new UserInRoleQM { UserID = _userId, Name = _roleName };
            var result = repo.IsUserInRole(qm);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        [Ignore]
        public void IsUserInRoleTest_False()
        {
            IdentityUserRepo repo = new IdentityUserRepo(connectionString);
            UserInRoleQM qm = new UserInRoleQM { UserID = _userId, Name = _roleName };
            var result = repo.IsUserInRole(qm);
            Assert.IsFalse(result = false);
        }

        [TestMethod()]
        [Ignore]
        public void RemoveFromRoleTest()
        {
            IdentityUserRepo repo = new IdentityUserRepo(connectionString);
            UserInRoleQM qm = new UserInRoleQM { UserID = _userId, Name = _roleName };
            IdentityUserRM user = new IdentityUserRM
            {
                UserName=_username,
                Id = _userId
            };
            repo.RemoveFromRole(user, qm);
        }

        [TestMethod()]
        [Ignore]
        public void UserHasPasswordTest_True()
        {
            IdentityUserRepo repo = new IdentityUserRepo(connectionString);
            UserIDQM qm = new UserIDQM { UserID = _userId };
            var result = repo.UserHasPassword(qm);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        [Ignore]
        public void UserHasPasswordTest_False()
        {
            IdentityUserRepo repo = new IdentityUserRepo(connectionString);
            UserIDQM qm = new UserIDQM { UserID = _userId };
            var result = repo.UserHasPassword(qm);
            Assert.IsFalse(result == false);
        }


        [TestMethod()]
        [Ignore]
        public void GetEmailConfirmedTest_True()
        {
            IdentityUserRepo repo = new IdentityUserRepo(connectionString);
            UserIDQM qm = new UserIDQM { UserID = _userId };
            var result = repo.GetEmailConfirmed(qm);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        [Ignore]
        public void GetEmailConfirmedTest_False()
        {
            IdentityUserRepo repo = new IdentityUserRepo(connectionString);
            UserIDQM qm = new UserIDQM { UserID = _userId };
            var result = repo.GetEmailConfirmed(qm);
            Assert.IsFalse(result = false);
        }


        [TestMethod()]
        [Ignore]
        public void CreateUserTest()
        {
            IdentityUserRepo repo = new IdentityUserRepo(connectionString);
            IdentityUserRM user = new IdentityUserRM
            {
                AccessFailedCount = 0,
                EmailAddress = "create@user.com",
                EmailConfirmed = false,
                PasswordHash = "hashthis",
                LockoutEnabled = false,
                LockoutEndDateUtc = DateTime.Now,
                UserName = "create@user.com"
            };
            repo.CreateUser(user);
        }

        [TestMethod()]
        [Ignore]
        public void UpdateUserTest()
        {
            IdentityUserRepo repo = new IdentityUserRepo(connectionString);
            IdentityUserRM user = new IdentityUserRM
            {
                Id = _userId,
                AccessFailedCount = 0,
                EmailAddress = "email@email.com",
                EmailConfirmed = false,
                PasswordHash = "asdfasdfasdfasdfasdfassdfasdf",
                LockoutEnabled = false,
                LockoutEndDateUtc = DateTime.Now,
                UserName = "email@email.com"
            };
            repo.UpdateUser(user);
        }

        [TestMethod()]
        [Ignore]
        public void DeleteUserTest()
        {
            IdentityUserRepo repo = new IdentityUserRepo(connectionString);
            UserIDQM qm = new UserIDQM { UserID = _userId };
            repo.DeleteUser(qm);
        }

        [TestMethod()]
        [Ignore]
        public void GetEmailAsyncTest()
        {
            IdentityUserRepo repo = new IdentityUserRepo(connectionString);
            string email = repo.GetEmailAsync(new UserIDQM { UserID = _userId }).ToString();
            Assert.IsTrue(email == _emailAddress);
        }

        [TestMethod()]
        [Ignore]
        public void AddUserToRoleTest()
        {
            IdentityUserDM dm = new IdentityUserDM
            {
                Id = _userId,
            };
            string roleName = _addRoleName;
            IdentityUserRepo repo = new IdentityUserRepo(connectionString);
            repo.AddUserToRole(dm, _addRoleName);
        }
    }
}