using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using PSPRS.Avengers.Core.Contracts;
using PSPRS.Avengers.DomainModels;
using AutoMapper;

namespace Avengers.MVC.Identity
{
    public class RoleStore : IRoleStore<IdentityRole, int>,
        IQueryableRoleStore<IdentityRole, int>
    {
        //protected IList<IdentityRole> roleList = new List<IdentityRole>();

        IIdentityRoleService roleService;

        public RoleStore (IIdentityRoleService roleService)
        {
            this.roleService = roleService;
        }

        public IQueryable<IdentityRole> Roles
        {
            get
            {
                IList<IdentityRoleDM> roleDMList = this.roleService.GetRoles();

                IList<IdentityRole> roleList = Mapper.Map<IList<IdentityRoleDM>, IList<IdentityRole>>(roleDMList);

                return roleList.AsQueryable();
            }
        }

        public Task CreateAsync(IdentityRole role)
        {
            IdentityRoleDM roleDM = Mapper.Map<IdentityRole, IdentityRoleDM>(role);

            this.roleService.CreateRole(roleDM);

            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(IdentityRole role)
        {
            //if (this.roleList.Contains(role))
            //{
            //    this.roleList.Remove(role);
            //}

            return Task.FromResult<object>(null);
        }

        public void Dispose()
        {
            
        }

        public Task<IdentityRole> FindByIdAsync(int roleId)
        {
            IdentityRoleDM model = this.roleService.GetRoleById(roleId);

            IdentityRole role = new IdentityRole()
            {
                Id = model.Id,
                Name = model.Name
            };

            return Task.FromResult<IdentityRole>(role);
        }

        public Task<IdentityRole> FindByNameAsync(string roleName)
        {
            IdentityRoleDM model = this.roleService.GetRoleByName(roleName);

            IdentityRole role = new IdentityRole()
            {
                Id = model.Id,
                Name = model.Name
            };

            return Task.FromResult<IdentityRole>(role);
        }

        public Task UpdateAsync(IdentityRole role)
        {
            IdentityRoleDM roleDM = Mapper.Map<IdentityRole, IdentityRoleDM>(role);

            this.roleService.UpdateRole(roleDM);

            return Task.FromResult<object>(null);
        }
    }
}