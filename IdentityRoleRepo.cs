using PSPRS.Avengers.Repository.Contracts.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSPRS.Avengers.DomainModels;
using Avengers.RepositoryModels.Identity;

namespace PSPRS.Avengers.Repository.Identity
{
    public class IdentityRoleRepo : SqlServerBase, IIdentityRoleRepo
    {
        public IdentityRoleRepo(string connectionString) : base(connectionString)
        {
        }

        public void CreateRole(IdentityRoleRM role)
        {
            int result;
            GetRoleNameQM qm = new GetRoleNameQM { Name = role.Name };
            using (this)
            {
                string storedProc = "identity_roles_create_role";
                result = ExecuteNonQuery(storedProc, qm);
            }
            if (result < 1)
            {
                throw new Exception("Failed to insert record");
            }
        }

        public void UpdateRole(IdentityRoleRM role)
        {
            int result;
            RoleUpdateQM qm = new RoleUpdateQM { RoleID = role.Id, Name = role.Name};
            using (this)
            {
                string storedProc = "identity_roles_update_role";
                result = ExecuteNonQuery(storedProc, qm);
            }
            if (result < 1)
            {
                throw new Exception("Failed to update record");
            }
        }

        public IdentityRoleRM GetRoleById(RoleIDQM queryModel)
        {
            using (this)
            {
                string storedProc = "identity_roles_get_role_by_id";
                return ExecuteReader<IdentityRoleRM, RoleIDQM>(storedProc, queryModel).FirstOrDefault();
            }
        }

        public IdentityRoleRM GetRoleByName(GetRoleNameQM queryModel)
        {
            using (this)
            {
                string storedProc = "identity_roles_get_role_by_name";
                return ExecuteReader<IdentityRoleRM, GetRoleNameQM>(storedProc, queryModel).FirstOrDefault();
            }
        }

        public List<IdentityRoleRM> GetRoles()
        {
            using (this)
            {
                string storedProc = "identity_roles_get_roles";
                return ExecuteReader<IdentityRoleRM>(storedProc);
            }
        }
    }
}
