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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().HasOptional(u => u.ChangePasswordToken).WithRequired(t => t.User).WillCascadeOnDelete(false);

            modelBuilder.Entity<Poll>().HasMany(p => p.Orders).WithRequired(o => o.Poll).WillCascadeOnDelete(false);
            modelBuilder.Entity<Group>().HasMany(g => g.ApplicationUsers).WithOptional(u => u.Group).WillCascadeOnDelete(false);

        }

        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Kind> Kinds { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
    }
}
