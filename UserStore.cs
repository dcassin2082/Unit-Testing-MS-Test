using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using PSPRS.Avengers.Exceptions;
using PSPRS.Avengers.Core.Contracts;
using PSPRS.Avengers.DomainModels;
using AutoMapper;

namespace Avengers.MVC.Identity
{
    public class UserStore : IUserStore<IdentityUser, int>,
        IUserRoleStore<IdentityUser, int>,
        IUserPasswordStore<IdentityUser, int>,
        IUserLockoutStore<IdentityUser, int>,
        IUserEmailStore<IdentityUser, int>,
        IQueryableUserStore<IdentityUser, int>,
        IUserSecurityStampStore<IdentityUser, int>
        //IUserSecurityStampStore<IdentityUser, int>
    {

        IRoleStore<IdentityRole, int> roleStore; // = new RoleStore ();
        IIdentityUserService userService;




        public UserStore()
        {

        }

        public UserStore(IRoleStore<IdentityRole, int> roleStore,
            IIdentityUserService userService)
        {
            this.roleStore = roleStore;
            this.userService = userService;
        }

        public IQueryable<IdentityUser> Users
        {
            get
            {
                IList<IdentityUserDM> userDMList = this.userService.GetUsers();
                IList<IdentityUser> userList = new List<IdentityUser>();

                foreach (var userDM in userDMList)
                {
                    userList.Add(
                        new IdentityUser()
                        {
                            Id = userDM.Id,
                            UserName = userDM.UserName,
                            EmailAddress = userDM.EmailAddress
                        });
                }

                return userList.AsQueryable();
            }
        }

        public Task AddToRoleAsync(IdentityUser user, string roleName)
        {
            IdentityUserDM userDM = Mapper.Map<IdentityUser, IdentityUserDM>(user);

            this.userService.AddToRole(userDM, roleName);

            return Task.FromResult<object>(null);
        }

        public Task CreateAsync(IdentityUser user)
        {
            IdentityUserDM userDM = Mapper.Map<IdentityUser, IdentityUserDM>(user);

            this.userService.CreateUser(userDM);

            return Task.FromResult(user);
        }

        //public Task ChangePasswordAsync(IdentityUser user, string currentPassword, string newPassword)
        //{
        //    IdentityUserDM userDM = Mapper.Map<IdentityUser, IdentityUserDM>(user);
        //    SetPasswordHashAsync(user, newPassword);
        //    userService.UpdateUser(userDM);
        //    return Task.FromResult(user);
        //}

        public Task DeleteAsync(IdentityUser user)
        {
            IdentityUserDM userDM = Mapper.Map<IdentityUser, IdentityUserDM>(user);

            this.userService.DeleteUser(userDM);

            return Task.FromResult<object>(null);
        }

        public void Dispose()
        {

        }

        public Task<IdentityUser> FindByIdAsync(int userId)
        {
            IdentityUserDM userDM = this.userService.GetUserById(userId);

            IdentityUser user = Mapper.Map<IdentityUserDM, IdentityUser>(userDM);

            return Task.FromResult<IdentityUser>(user);
        }

        public Task<IdentityUser> FindByNameAsync(string userName)
        {
            IdentityUserDM userDM = this.userService.GetUserByName(userName);

            IdentityUser user = Mapper.Map<IdentityUserDM, IdentityUser>(userDM);

            return Task.FromResult<IdentityUser>(user);
        }

        public Task<IList<string>> GetRolesAsync(IdentityUser user)
        {
            IdentityUserDM userDM = Mapper.Map<IdentityUser, IdentityUserDM>(user);

            var list = this.userService.GetUserRoles(userDM);

            return Task.FromResult<IList<string>>(list);
        }

        public Task<bool> IsInRoleAsync(IdentityUser user, string roleName)
        {
            IdentityUserDM userDM = Mapper.Map<IdentityUser, IdentityUserDM>(user);

            bool isinrole = this.userService.IsUserInRole(userDM, roleName);

            return Task.FromResult<bool>(isinrole);
        }

        public Task RemoveFromRoleAsync(IdentityUser user, string roleName)
        {
            IdentityUserDM userDM = Mapper.Map<IdentityUser, IdentityUserDM>(user);

            this.userService.RemoveFromRole(userDM, roleName);

            return Task.FromResult<object>(null);
        }

        public Task UpdateAsync(IdentityUser user)
        {
            IdentityUserDM userDM = Mapper.Map<IdentityUser, IdentityUserDM>(user);
            userService.UpdateUser(userDM);
            
            return Task.FromResult<object>(null);
        }

        public Task SetPasswordHashAsync(IdentityUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;

            UpdateAsync(user);

            return Task.FromResult<object>(null);
        }

        public Task GeneratePasswordResetTokenAsync(int userId)
        {
            var result = FindByIdAsync(userId);
            IdentityUser user = result.Result;
            return Task.FromResult<object>(null);
        }

        public Task<string> GetPasswordHashAsync(IdentityUser user)
        {
            return Task.FromResult<string>(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(IdentityUser user)
        {
            IdentityUserDM userDM = Mapper.Map<IdentityUser, IdentityUserDM>(user);

            bool hasPassord = this.userService.UserHasPassword(userDM);

            return Task.FromResult(hasPassord);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(IdentityUser user)
        {
            return
                Task.FromResult(user.LockoutEndDateUtc.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                    : new DateTimeOffset());
        }

        public Task SetLockoutEndDateAsync(IdentityUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            UpdateAsync(user);

            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(IdentityUser user)
        {
            user.AccessFailedCount++;
            UpdateAsync(user);

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(IdentityUser user)
        {
            user.AccessFailedCount = 0;
            UpdateAsync(user);

            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(IdentityUser user)
        {
            return Task.FromResult(0);
        }

        public Task<bool> GetLockoutEnabledAsync(IdentityUser user)
        {
            return Task.FromResult<bool>(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(IdentityUser user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            UpdateAsync(user);

            return Task.FromResult(0);
        }

        public Task SetEmailAsync(IdentityUser user, string email)
        {
            user.EmailAddress = email;
            UpdateAsync(user);

            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(IdentityUser user)
        {
            IdentityUserDM userDM = Mapper.Map<IdentityUser, IdentityUserDM>(user);

            string email = this.userService.GetEmailAsync(userDM);

            return Task.FromResult(email);
        }

        public Task<bool> GetEmailConfirmedAsync(IdentityUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            UpdateAsync(user);

            return Task.FromResult(0);
        }

        public Task<IdentityUser> FindByEmailAsync(string email)
        {
            IdentityUserDM userDM = this.userService.GetUserByEmail(email);

            IdentityUser user = Mapper.Map<IdentityUserDM, IdentityUser>(userDM);

            return Task.FromResult(user);
        }

        public Task SetSecurityStampAsync(IdentityUser user, string stamp)
        {
            user.SecurityStamp = stamp;
            UpdateAsync(user);
            return Task.FromResult(Guid.NewGuid());
        }

        public Task<string> GetSecurityStampAsync(IdentityUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

    }
}