using AutoMapper;
using Avengers.RepositoryModels.Identity;
using PSPRS.Avengers.Core.Contracts;
using PSPRS.Avengers.DomainModels;
using PSPRS.Avengers.Repository.Contracts.Identity;
using System;
using System.Collections.Generic;

namespace PSPRS.Avengers.Core
{
    public class IdentityService : IIdentityRoleService, IIdentityUserService
    {
        IIdentityRoleRepo identityRoleRepo;
        IIdentityUserRepo identityUserRepo;

        #region Constructors
        public IdentityService()
        {
        }

        public IdentityService(IIdentityRoleRepo identityRoleRepo, IIdentityUserRepo identityUserRepo)
        {
            this.identityRoleRepo = identityRoleRepo;
            this.identityUserRepo = identityUserRepo;
        }
        
        public IdentityService(IIdentityUserRepo identityUserRepo)
        {
            this.identityUserRepo = identityUserRepo;
        }

        public IdentityService(IIdentityRoleRepo identityRoleRepo)
        {
            this.identityRoleRepo = identityRoleRepo;
        }
        #endregion

        
        public void AddToRole(IdentityUserDM user, string roleName)
        {
            identityUserRepo.AddUserToRole(user, roleName);
        }

        public void CreateUser(IdentityUserDM user)
        {
            identityUserRepo.CreateUser(Mapper.Map<IdentityUserDM, IdentityUserRM>(user));
        }

        public void DeleteUser(IdentityUserDM user)
        {
            identityUserRepo.DeleteUser(new UserIDQM { UserID = user.Id });
        }

        public IdentityUserDM GetUserById(int userId)
        {
            IdentityUserRM user = identityUserRepo.GetUserById(new UserIDQM { UserID = userId });
            return Mapper.Map<IdentityUserRM, IdentityUserDM>(user);
        }

        public IdentityUserDM GetUserByName(string userName)
        {
            IdentityUserRM user = identityUserRepo.GetUserByUserName(new GetUserUserNameQM { UserName = userName });
            return Mapper.Map<IdentityUserRM, IdentityUserDM>(user);
        }

        public IdentityUserDM GetUserByEmail(string email)
        {
            IdentityUserRM user = identityUserRepo.GetUserByEmailAddress(new GetUserEmailAddressQM { EmailAddress = email });
            return Mapper.Map<IdentityUserRM, IdentityUserDM>(user);
        }

        public IList<string> GetUserRoles(IdentityUserDM user)
        {
            return identityUserRepo.GetUserRoles(new UserIDQM { UserID = user.Id });
        }

        public bool UserHasPassword(IdentityUserDM user)
        {
            return identityUserRepo.UserHasPassword(new UserIDQM { UserID = user.Id });
        }

        public string GetEmailAsync(IdentityUserDM user)
        {
            return identityUserRepo.GetEmailAsync(new UserIDQM { UserID = user.Id });
        }

        public bool GetEmailConfirmed(IdentityUserDM user)
        {
            return identityUserRepo.GetEmailConfirmed(new UserIDQM { UserID = user.Id });
        }

        public IList<IdentityUserDM> GetUsers()
        {
            IList<IdentityUserRM> users = identityUserRepo.GetUsers();
            return Mapper.Map<IList<IdentityUserRM>, IList<IdentityUserDM>>(users);
        }

        public void UpdateUser(IdentityUserDM user)
        {
            IdentityUserRM userRM = Mapper.Map<IdentityUserDM, IdentityUserRM>(user);
            identityUserRepo.UpdateUser(userRM);
        }

        public void RemoveFromRole(IdentityUserDM user, string roleName)
        {
            IdentityUserRM userRM = Mapper.Map<IdentityUserDM, IdentityUserRM>(user);
            identityUserRepo.RemoveFromRole(userRM, new UserInRoleQM { Name = roleName, UserID = user.Id });
        }

        public bool IsUserInRole(IdentityUserDM user, string roleName)
        {
            return identityUserRepo.IsUserInRole(new UserInRoleQM { UserID = user.Id, Name = roleName });
        }

        public void CreateRole(IdentityRoleDM role)
        {
            identityRoleRepo.CreateRole(Mapper.Map<IdentityRoleDM, IdentityRoleRM>(role));
        }

        public IdentityRoleDM GetRoleById(int roleID)
        {
            IdentityRoleRM role = identityRoleRepo.GetRoleById(new RoleIDQM { RoleID = roleID });
            return Mapper.Map<IdentityRoleRM, IdentityRoleDM>(role);
        }

        public IdentityRoleDM GetRoleByName(string roleName)
        {
            IdentityRoleRM role = identityRoleRepo.GetRoleByName(new GetRoleNameQM { Name = roleName });
            return Mapper.Map<IdentityRoleRM, IdentityRoleDM>(role);
        }

        public IList<IdentityRoleDM> GetRoles()
        {
            IList<IdentityRoleRM> roles = identityRoleRepo.GetRoles();
            return Mapper.Map<IList<IdentityRoleRM>, IList<IdentityRoleDM>>(roles);
        }

        public void UpdateRole(IdentityRoleDM role)
        {
            IdentityRoleRM roleRM = Mapper.Map<IdentityRoleDM, IdentityRoleRM>(role);
            identityRoleRepo.UpdateRole(roleRM);
        }

        #region TestData
        //public IdentityService()
        //{
        //    this.userRoleList.Add(
        //        new UserRoleDM()
        //        {
        //            UserID = 1,
        //            Role = roleList[0]
        //        });

        //    this.userRoleList.Add(
        //        new UserRoleDM()
        //        {
        //            UserID = 2,
        //            Role = roleList[1]
        //        });
        //}

        //protected IList<IdentityUserDM> userList = new List<IdentityUserDM>()
        //{
        //    new IdentityUserDM()
        //    {
        //        Id = 1,
        //        UserName = "Joe Smith",
        //        EmailAddress = "Demo@email.com",
        //        PasswordHash = "AFkXksWcK040YSyh3apI4+ygk8L5VHB3Qtf4r3YHOy9YDwwQtb5YrweJCYGlVTvplA=="
        //    },
        //    new IdentityUserDM()
        //    {
        //        Id = 2,
        //        UserName = "Billy Bob",
        //        EmailAddress = "emp@email.com",
        //        PasswordHash = "AFkXksWcK040YSyh3apI4+ygk8L5VHB3Qtf4r3YHOy9YDwwQtb5YrweJCYGlVTvplA=="
        //    }
        //};

        //protected IList<IdentityRoleDM> roleList = new List<IdentityRoleDM>()
        //{
        //    new IdentityRoleDM() { Id = 1, Name = "Demographic" },
        //    new IdentityRoleDM() { Id = 2, Name = "EmployeeLookup" }
        //};

        //protected IList<UserRoleDM> userRoleList = new List<UserRoleDM>();

        #endregion
    }
}
