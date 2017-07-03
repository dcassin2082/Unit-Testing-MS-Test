using Avengers.RepositoryModels.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSPRS.Avengers.Repository.Tests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PSPRS.Avengers.Repository.Identity.Tests
{
    [TestClass()]
    public class IdentityRoleRepoTests : BaseRepoTest
    {
        private static int _roleId;
        private static string _roleName;
        private static string _updateRoleName;

        [TestInitialize()]
        public void Init()
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                string sql = "insert into identity_roles(name) values('Test Role')";
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("select * from identity_roles where name = 'Test Role'", cn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    _roleId = GetFieldValue<int>(dt.Rows[0], "role_id");
                    _roleName = dt.Rows[0]["name"].ToString();
                }
            }
        }

        [TestCleanup()]
        public void Cleanup()
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                string sql = string.Format("delete from identity_roles where name='{0}' or name ='{1}'", _roleName, _updateRoleName);
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            };
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

        [TestMethod()]
        public void CreateRoleTest()
        {
            IdentityRoleRM role = new IdentityRoleRM
            {
                Name = "Create Role"
            };
            _updateRoleName = role.Name;
            IdentityRoleRepo repo = new IdentityRoleRepo(connectionString);
            repo.CreateRole(role);
        }

        [TestMethod()]
        [Ignore]
        public void UpdateRoleTest()
        {
            _updateRoleName = "Update role";
            IdentityRoleRM role = new IdentityRoleRM(_roleId, _updateRoleName);
            IdentityRoleRepo repo = new IdentityRoleRepo(connectionString);
            repo.UpdateRole(role);
        }

        [TestMethod()]
        [Ignore]
        public void GetRoleByIdTest()
        {
            IdentityRoleRepo repo = new IdentityRoleRepo(connectionString);
            RoleIDQM qm = new RoleIDQM { RoleID = _roleId };
            IdentityRoleRM result = repo.GetRoleById(qm);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        [Ignore]

        public void GetRoleByNameTest()
        {
            IdentityRoleRepo repo = new IdentityRoleRepo(connectionString);
            GetRoleNameQM qm = new GetRoleNameQM { Name = _roleName };
            IdentityRoleRM result = repo.GetRoleByName(qm);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, qm.Name);
        }

        [TestMethod()]
        [Ignore]
        public void GetRolesTest()
        {
            IdentityRoleRepo repo = new IdentityRoleRepo(connectionString);
            List<IdentityRoleRM> results = repo.GetRoles();
            Assert.IsTrue(results.Count > 0);
        }
    }
}