using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avengers.MVC.Identity
{
    public class IdentityRole : IRole<int>
    {
        public IdentityRole()
        { }

        public IdentityRole(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}