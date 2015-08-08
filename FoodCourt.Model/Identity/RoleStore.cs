using System;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FoodCourt.Model.Identity
{
    public class RoleStore : RoleStore<Role, Guid, UserRole>
    {
        public RoleStore(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}