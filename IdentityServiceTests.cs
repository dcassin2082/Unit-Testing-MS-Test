using AutoMapper;
using Avengers.RepositoryModels.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSPRS.Avengers.DomainModels;
using PSPRS.Avengers.Repository.Contracts.Identity;
using PSPRS.Avengers.Repository.Contracts.Identity.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PSPRS.Avengers.Core.Tests
{
    [TestClass()]
    public class IdentityServiceTests
    {
        [TestInitialize()]
        public void Init()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<IdentityUserRM, IdentityUserDM>();
                config.CreateMap<IdentityRoleRM, IdentityRoleDM>();
                config.CreateMap<IdentityUserDM, IdentityUserRM>();
                config.CreateMap<IdentityRoleDM, IdentityRoleRM>();
            });
        }

        [TestMethod()]
        public void AddToRoleTest()
        {
            IdentityUserDM domainModel = new IdentityUserDM
            {
                Id = 1,
                UserName = "user",
                PasswordHash = null,
                EmailAddress = "user@asdf.com",
                AccessFailedCount = 0,
                EmailConfirmed = false,
                LockoutEnabled = false,
                LockoutEndDateUtc = DateTime.Now
            };
            string role = null;
            string roleName = "Admin";
            IIdentityUserRepo repo = new StubIIdentityUserRepo()
            {
                AddUserToRoleIdentityUserDMString = (passedDM, passedString) =>
                {
                    domainModel = passedDM;
                    role = passedString;
                }
            };
            IdentityService service = new IdentityService(repo);
            service.AddToRole(domainModel, roleName);
            Assert.AreEqual(role, roleName);
        }

        [TestMethod()]
        public void CreateUserTest()
        {
            IdentityUserRM repoModel = new IdentityUserRM
            {
                Id = 1,
                UserName = "user",
                PasswordHash = null,
                EmailAddress = "user@asdf.com",
                AccessFailedCount = 0,
                EmailConfirmed = false,
                LockoutEnabled = false,
                LockoutEndDateUtc = DateTime.Now
            };
            IdentityUserDM domainModel = null;
            IIdentityUserRepo repo = new StubIIdentityUserRepo()
            {
                CreateUserIdentityUserRM = (passedRM) =>
                {
                    domainModel = Mapper.Map<IdentityUserRM, IdentityUserDM>(passedRM);
                }
            };
            IdentityService service = new IdentityService(repo);
            service.CreateUser(Mapper.Map<IdentityUserRM, IdentityUserDM>(repoModel));
            Assert.AreEqual(domainModel.Id, repoModel.Id);
            Assert.AreEqual(domainModel.UserName, repoModel.UserName);
            Assert.AreEqual(domainModel.PasswordHash, repoModel.PasswordHash);
            Assert.AreEqual(domainModel.AccessFailedCount, repoModel.AccessFailedCount);
            Assert.AreEqual(domainModel.EmailConfirmed, repoModel.EmailConfirmed);
            Assert.AreEqual(domainModel.LockoutEnabled, repoModel.LockoutEnabled);
            Assert.AreEqual(domainModel.LockoutEndDateUtc, repoModel.LockoutEndDateUtc);
        }

        [TestMethod()]
        public void DeleteUserTest()
        {
            IdentityUserRM repoModel = new IdentityUserRM
            {
                Id = 1,
                UserName = "user",
                PasswordHash = null,
                EmailAddress = "user@asdf.com",
                AccessFailedCount = 0,
                EmailConfirmed = false,
                LockoutEnabled = false,
                LockoutEndDateUtc = DateTime.Now
            };
            UserIDQM queryModel = null;
            IIdentityUserRepo repo = new StubIIdentityUserRepo
            {
                DeleteUserUserIDQM = (passedQM) =>
                {
                    queryModel = passedQM;
                }
            };
            IdentityService service = new IdentityService(repo);
            service.DeleteUser(Mapper.Map<IdentityUserRM, IdentityUserDM>(repoModel));
            Assert.AreEqual(queryModel.UserID, repoModel.Id);
        }

        [TestMethod()]
        public void GetUserByIdTest()
        {
            IdentityUserRM repoModel = new IdentityUserRM
            {
                Id = 1,
                UserName = "user",
                PasswordHash = null,
                EmailAddress = "user@asdf.com",
                AccessFailedCount = 0,
                EmailConfirmed = false,
                LockoutEnabled = false,
                LockoutEndDateUtc = DateTime.Now
            };
            UserIDQM queryModel = null;
            IIdentityUserRepo repo = new StubIIdentityUserRepo
            {
                GetUserByIdUserIDQM = (passedQM) =>
                {
                    queryModel = passedQM;
                    return repoModel;
                }
            };
            IdentityService service = new IdentityService(repo);
            var result = service.GetUserById(repoModel.Id);
            Assert.AreEqual(result.Id, repoModel.Id);
            Assert.AreEqual(result.UserName, repoModel.UserName);
            Assert.AreEqual(result.PasswordHash, repoModel.PasswordHash);
            Assert.AreEqual(result.EmailAddress, repoModel.EmailAddress);
            Assert.AreEqual(result.AccessFailedCount, repoModel.AccessFailedCount);
            Assert.AreEqual(result.EmailConfirmed, repoModel.EmailConfirmed);
            Assert.AreEqual(result.LockoutEnabled, repoModel.LockoutEnabled);
            Assert.AreEqual(result.LockoutEndDateUtc, repoModel.LockoutEndDateUtc);
        }

        [TestMethod()]
        public void GetUserByNameTest()
        {
            IdentityUserRM repoModel = new IdentityUserRM
            {
                Id = 1,
                UserName = "user",
                PasswordHash = null,
                EmailAddress = "user@asdf.com",
                AccessFailedCount = 0,
                EmailConfirmed = false,
                LockoutEnabled = false,
                LockoutEndDateUtc = DateTime.Now
            };
            GetUserUserNameQM queryModel = null;
            IIdentityUserRepo repo = new StubIIdentityUserRepo
            {
                GetUserByUserNameGetUserUserNameQM = (passedQM) =>
                {
                    queryModel = passedQM;
                    return repoModel;
                }
            };
            IdentityService service = new IdentityService(repo);
            var result = service.GetUserByName(repoModel.UserName);
            Assert.AreEqual(result.Id, repoModel.Id);
            Assert.AreEqual(result.UserName, repoModel.UserName);
            Assert.AreEqual(result.PasswordHash, repoModel.PasswordHash);
            Assert.AreEqual(result.EmailAddress, repoModel.EmailAddress);
            Assert.AreEqual(result.AccessFailedCount, repoModel.AccessFailedCount);
            Assert.AreEqual(result.EmailConfirmed, repoModel.EmailConfirmed);
            Assert.AreEqual(result.LockoutEnabled, repoModel.LockoutEnabled);
            Assert.AreEqual(result.LockoutEndDateUtc, repoModel.LockoutEndDateUtc);
        }

        [TestMethod()]
        public void GetUserByEmailTest()
        {
            IdentityUserRM repoModel = new IdentityUserRM
            {
                Id = 1,
                UserName = "user@asdf.com",
                PasswordHash = null,
                EmailAddress = "user@asdf.com",
                AccessFailedCount = 0,
                EmailConfirmed = false,
                LockoutEnabled = false,
                LockoutEndDateUtc = DateTime.Now
            };
            GetUserEmailAddressQM queryModel = null;
            IIdentityUserRepo repo = new StubIIdentityUserRepo
            {
                GetUserByEmailAddressGetUserEmailAddressQM = (passedQm) =>
                {
                    queryModel = passedQm;
                    return repoModel;
                }
            };
            IdentityService service = new IdentityService(repo);
            var result = service.GetUserByEmail(repoModel.EmailAddress);
            Assert.AreEqual(result.Id, repoModel.Id);
            Assert.AreEqual(result.UserName, repoModel.UserName);
            Assert.AreEqual(result.PasswordHash, repoModel.PasswordHash);
            Assert.AreEqual(result.EmailAddress, repoModel.EmailAddress);
            Assert.AreEqual(result.AccessFailedCount, repoModel.AccessFailedCount);
            Assert.AreEqual(result.EmailConfirmed, repoModel.EmailConfirmed);
            Assert.AreEqual(result.LockoutEnabled, repoModel.LockoutEnabled);
            Assert.AreEqual(result.LockoutEndDateUtc, repoModel.LockoutEndDateUtc);
        }

        [TestMethod()]
        public void GetUserRolesTest()
        {
            IdentityUserDM user = new IdentityUserDM
            {
                Id = 1,
                UserName = "user@asdf.com",
                PasswordHash = null,
                EmailAddress = "user@asdf.com",
                AccessFailedCount = 0,
                EmailConfirmed = false,
                LockoutEnabled = false,
                LockoutEndDateUtc = DateTime.Now
            };
            IList<string> userRoles = new List<string>();
            userRoles.Add("Admin");
            UserIDQM queryModel = null;
            IIdentityUserRepo repo = new StubIIdentityUserRepo
            {
                GetUserRolesUserIDQM = (passedQM) =>
                {
                    queryModel = passedQM;
                    return userRoles;
                }
            };
            IdentityService service = new IdentityService(repo);
            var results = service.GetUserRoles(user);
            Assert.IsTrue(results.Count > 0);
            Assert.AreEqual(results, userRoles);
        }

        [TestMethod()]
        public void UserHasPasswordTest()
        {
            IdentityUserDM domainModel = new IdentityUserDM
            {
                Id = 1,
                UserName = "user@asdf.com",
                PasswordHash = "password",
                EmailAddress = "user@asdf.com",
                AccessFailedCount = 0,
                EmailConfirmed = false,
                LockoutEnabled = false,
                LockoutEndDateUtc = DateTime.Now
            };
            UserIDQM queryModel = null;
            IIdentityUserRepo repo = new StubIIdentityUserRepo
            {
                UserHasPasswordUserIDQM = (passedQM) =>
                 {
                     queryModel = passedQM;
                     return !string.IsNullOrEmpty(domainModel.PasswordHash);
                 }
            };
            IdentityService service = new IdentityService(repo);
            var result = service.UserHasPassword(domainModel);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void GetEmailAsyncTest()
        {
            IdentityUserDM domainModel = new IdentityUserDM
            {
                Id = 1,
                UserName = "user@asdf.com",
                EmailAddress = "asdf@asdf.com"
            };
            UserIDQM queryModel = null;
            IIdentityUserRepo repo = new StubIIdentityUserRepo
            {
                GetEmailAsyncUserIDQM = (passedQM) =>
                {
                    queryModel = passedQM;
                    return domainModel.EmailAddress;
                }
            };
            IdentityService service = new IdentityService(repo);
            var result = service.GetEmailAsync(domainModel);
            Assert.AreEqual(result, domainModel.EmailAddress);
        }

        [TestMethod()]
        public void GetEmailConfirmedTest()
        {
            IdentityUserDM domainModel = new IdentityUserDM
            {
                Id = 1,
                UserName = "user@asdf.com",
                PasswordHash = "password",
                EmailAddress = "user@asdf.com",
                AccessFailedCount = 0,
                EmailConfirmed = true,
                LockoutEnabled = false,
                LockoutEndDateUtc = DateTime.Now
            };
            UserIDQM queryModel = null;
            IIdentityUserRepo repo = new StubIIdentityUserRepo
            {
                GetEmailConfirmedUserIDQM = (passedQM) =>
                {
                    queryModel = passedQM;
                    return domainModel.EmailConfirmed;
                }
            };
            IdentityService service = new IdentityService(repo);
            var result = service.GetEmailConfirmed(domainModel);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void GetUsersTest()
        {
            IList<IdentityUserRM> users = new List<IdentityUserRM>
            {
                new IdentityUserRM
                {
                    Id = 1,
                    UserName = "user@asdf.com",
                    PasswordHash = "password",
                    EmailAddress = "user@asdf.com",
                    AccessFailedCount = 0,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    LockoutEndDateUtc = DateTime.Now
                },
                new IdentityUserRM
                {
                    Id = 2,
                    UserName = "blah@blahblah.com",
                    PasswordHash = "password",
                    EmailAddress = "blah@asdf.com",
                    AccessFailedCount = 0,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    LockoutEndDateUtc = DateTime.Now
                }
            };
            IIdentityUserRepo repo = new StubIIdentityUserRepo
            {
                GetUsers = () =>
                {
                    return users.ToList();
                }
            };
            IdentityService service = new IdentityService(repo);
            var results = service.GetUsers();
            Assert.IsTrue(results.Count == 2);
            for(int i = 0; i < results.Count; i++)
            {
                Assert.AreEqual(results[i].Id, users[i].Id);
                Assert.AreEqual(results[i].UserName, users[i].UserName);
                Assert.AreEqual(results[i].PasswordHash, users[i].PasswordHash);
                Assert.AreEqual(results[i].EmailAddress, users[i].EmailAddress);
                Assert.AreEqual(results[i].AccessFailedCount, users[i].AccessFailedCount);
                Assert.AreEqual(results[i].EmailConfirmed, users[i].EmailConfirmed);
                Assert.AreEqual(results[i].LockoutEnabled, users[i].LockoutEnabled);
                Assert.AreEqual(results[i].LockoutEndDateUtc, users[i].LockoutEndDateUtc);
            }
        }

        [TestMethod()]
        public void UpdateUserTest()
        {
            IdentityRoleRM repoModel = new IdentityRoleRM
            {
                Id = 1111,
                Name = "admin"
            };
            IdentityRoleDM domainModel = null;
            IIdentityRoleRepo repo = new StubIIdentityRoleRepo()
            {
                UpdateRoleIdentityRoleRM = (passedRM) =>
                {
                    repoModel = passedRM;
                    domainModel = Mapper.Map<IdentityRoleRM, IdentityRoleDM>(passedRM);
                }
            };
            IdentityService service = new IdentityService(repo);
            service.UpdateRole(Mapper.Map<IdentityRoleRM, IdentityRoleDM>(repoModel));
            Assert.IsNotNull(domainModel);
            Assert.AreEqual(domainModel.Name, repoModel.Name);
        }

        [TestMethod()]
        public void RemoveFromRoleTest()
        {
            IdentityUserDM domainModel = new IdentityUserDM
            {
                Id = 1,
                UserName = "user",
                PasswordHash = null,
                EmailAddress = "user@asdf.com",
                AccessFailedCount = 0,
                EmailConfirmed = false,
                LockoutEnabled = false,
                LockoutEndDateUtc = DateTime.Now
            };
            string role = null;
            string roleName = "Admin";
            UserInRoleQM queryModel = new UserInRoleQM { Name = roleName, UserID = domainModel.Id };
            IIdentityUserRepo repo = new StubIIdentityUserRepo()
            {
                RemoveFromRoleIdentityUserRMUserInRoleQM = (passedDM, passedString) =>
                {
                    domainModel = Mapper.Map<IdentityUserRM, IdentityUserDM>(passedDM);
                    role = queryModel.Name;
                }
            };
            IdentityService service = new IdentityService(repo);
            service.RemoveFromRole(domainModel, roleName);
            Assert.AreEqual(role, roleName);
        }

        [TestMethod()]
        public void IsUserInRoleTest()
        {
            IdentityUserDM domainModel = new IdentityUserDM
            {
                Id = 1,
                UserName = "user@asdf.com",
                PasswordHash = "password",
                EmailAddress = "user@asdf.com",
                AccessFailedCount = 0,
                EmailConfirmed = false,
                LockoutEnabled = false,
                LockoutEndDateUtc = DateTime.Now
            };

            UserInRoleQM queryModel = null;
            string roleName = "Admin";
            IIdentityUserRepo repo = new StubIIdentityUserRepo
            {
                IsUserInRoleUserInRoleQM = (passedQM) =>
                {
                    queryModel = passedQM;
                    return queryModel.Name.Equals(roleName);
                }
            };
            IdentityService service = new IdentityService(repo);
            var result = service.IsUserInRole(domainModel, roleName);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CreateRoleTest()
        {
            IdentityRoleRM repoModel = new IdentityRoleRM
            {
                Id = 1,
                Name = "New Role Tester"
            };
            IdentityRoleDM domainModel = null;
            IIdentityRoleRepo repo = new StubIIdentityRoleRepo
            {
                CreateRoleIdentityRoleRM = (passedRM) =>
                {
                    domainModel = Mapper.Map<IdentityRoleRM, IdentityRoleDM>(passedRM);
                }
            };
            IdentityService service = new IdentityService(repo);
            service.CreateRole(Mapper.Map<IdentityRoleRM, IdentityRoleDM>(repoModel));
            Assert.AreEqual(repoModel.Id, domainModel.Id);
            Assert.AreEqual(repoModel.Name, domainModel.Name);
        }

        [TestMethod()]
        public void GetRoleByIdTest()
        {
            IdentityRoleRM repoModel = new IdentityRoleRM
            {
                Id = 1,
                Name = "Admin"
            };
            RoleIDQM queryModel = null;
            IIdentityRoleRepo repo = new StubIIdentityRoleRepo
            {
                GetRoleByIdRoleIDQM = (passedQM) =>
                {
                    queryModel = passedQM;
                    return repoModel;
                }
            };
            IdentityService service = new IdentityService(repo);
            var result = service.GetRoleById(repoModel.Id);
            Assert.AreEqual(result.Id, repoModel.Id);
            Assert.AreEqual(result.Name, repoModel.Name);
        }

        [TestMethod()]
        public void GetRoleByNameTest()
        {
            IdentityRoleRM repoModel = new IdentityRoleRM
            {
                Id = 1,
                Name = "Admin"
            };
            GetRoleNameQM queryModel = null;
            IIdentityRoleRepo repo = new StubIIdentityRoleRepo
            {
                GetRoleByNameGetRoleNameQM = (passedQM) =>
                {
                    queryModel = passedQM;
                    return repoModel;
                }
            };
            IdentityService service = new IdentityService(repo);
            var result = service.GetRoleByName(repoModel.Name);
            Assert.AreEqual(result.Id, repoModel.Id);
            Assert.AreEqual(result.Name, repoModel.Name);
        }

        [TestMethod()]
        public void GetRolesTest()
        {
            IList<IdentityRoleRM> roles = new List<IdentityRoleRM>
            {
                new IdentityRoleRM
                {
                    Id = 1,
                    Name="Admin"
                },
                new IdentityRoleRM
                {
                    Id = 2,
                    Name = "Tester"
                }
            };
            IIdentityRoleRepo repo = new StubIIdentityRoleRepo
            {
                GetRoles = () =>
                {
                    return roles.ToList();
                }
            };
            IdentityService service = new IdentityService(repo);
            var results = service.GetRoles();
            Assert.IsTrue(results.Count == 2);
            for(int i = 0; i < results.Count; i++)
            {
                Assert.AreEqual(results[i].Id, roles[i].Id);
                Assert.AreEqual(results[i].Name, roles[i].Name);
            }
        }

        [TestMethod()]
        public void UpdateRoleTest()
        {
            IdentityRoleDM domainModel = null;
            IdentityRoleRM repoModel = new IdentityRoleRM
            {
                Id = 1111,
                Name = "admin"
            };
            IIdentityRoleRepo repo = new StubIIdentityRoleRepo()
            {
                UpdateRoleIdentityRoleRM = (passedRM) =>
                {
                    repoModel = passedRM;
                    domainModel = Mapper.Map<IdentityRoleRM, IdentityRoleDM>(passedRM);
                }
            };
            IdentityService service = new IdentityService(repo);
            service.UpdateRole(Mapper.Map<IdentityRoleRM, IdentityRoleDM>(repoModel));
            Assert.IsNotNull(domainModel);
            Assert.AreEqual(domainModel.Name, repoModel.Name);
            Assert.AreEqual(domainModel.Id, repoModel.Id);

        }
    }
}