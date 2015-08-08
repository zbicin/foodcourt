using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodCourt.Model.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FoodCourt.Model
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, Role, Guid, UserLogin, UserRole, UserClaim>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Kind> Kinds { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
    }
}
