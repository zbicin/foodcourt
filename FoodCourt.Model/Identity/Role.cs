using System;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FoodCourt.Model.Identity
{
    public class Role : IdentityRole<Guid, UserRole>
    {
        public Role()
        {
            Id = Guid.NewGuid();
        }
        public Role(string name) : this() { Name = name; }
    }
}