using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FoodCourt.Model.Identity
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

    public class ApplicationUser : IdentityUser<Guid, UserLogin, UserRole, UserClaim>, IApplicationUser
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        public ApplicationUser(string username)
            : this()
        {
            UserName = username;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, Guid> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public DateTime? LastOrderDate
        {
            get;
            set;
        }

        public Group Group
        {
            get;
            set;
        }

        public DateTime CreatedAt { get; set; }
        public ApplicationUser CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public ApplicationUser UpdatedBy { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public ApplicationUser DeletedBy { get; set; }

        public UserChangePasswordToken ChangePasswordToken { get; set; }
    }
}