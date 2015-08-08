using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using FoodCourt.Model.Identity;
using Microsoft.AspNet.Identity;

namespace FoodCourt.Service.Repository
{
    public interface IApplicationUserRepository
    {
        ApplicationUser GetSystemUser();
        DbContext Context { get; }
        bool DisposeContext { get; set; }
        bool AutoSaveChanges { get; set; }
        IQueryable<ApplicationUser> Users { get; }
        Task<ApplicationUser> Single(object primaryKey, bool withDeleted = false);
        IQueryable<ApplicationUser> GetAll(bool withDeleted = false, string includes = "");
        Task<bool> Exists(object primaryKey, bool withDeleted = false);
        Task<int> Insert(ApplicationUser entity);
        Task<int> Update(ApplicationUser entity);
        Task<int> Delete(ApplicationUser entity);
        Task<IList<Claim>> GetClaimsAsync(ApplicationUser user);
        Task AddClaimAsync(ApplicationUser user, Claim claim);
        Task RemoveClaimAsync(ApplicationUser user, Claim claim);
        Task<bool> GetEmailConfirmedAsync(ApplicationUser user);
        Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed);
        Task SetEmailAsync(ApplicationUser user, string email);
        Task<string> GetEmailAsync(ApplicationUser user);
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user);
        Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd);
        Task<int> IncrementAccessFailedCountAsync(ApplicationUser user);
        Task ResetAccessFailedCountAsync(ApplicationUser user);
        Task<int> GetAccessFailedCountAsync(ApplicationUser user);
        Task<bool> GetLockoutEnabledAsync(ApplicationUser user);
        Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled);
        Task<ApplicationUser> FindByIdAsync(Guid userId);
        Task<ApplicationUser> FindByNameAsync(string userName);
        Task CreateAsync(ApplicationUser user);
        Task DeleteAsync(ApplicationUser user);
        Task UpdateAsync(ApplicationUser user);
        void Dispose();
        Task<ApplicationUser> FindAsync(UserLoginInfo login);
        Task AddLoginAsync(ApplicationUser user, UserLoginInfo login);
        Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login);
        Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user);
        Task SetPasswordHashAsync(ApplicationUser user, string passwordHash);
        Task<string> GetPasswordHashAsync(ApplicationUser user);
        Task<bool> HasPasswordAsync(ApplicationUser user);
        Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber);
        Task<string> GetPhoneNumberAsync(ApplicationUser user);
        Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user);
        Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed);
        Task AddToRoleAsync(ApplicationUser user, string roleName);
        Task RemoveFromRoleAsync(ApplicationUser user, string roleName);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task<bool> IsInRoleAsync(ApplicationUser user, string roleName);
        Task SetSecurityStampAsync(ApplicationUser user, string stamp);
        Task<string> GetSecurityStampAsync(ApplicationUser user);
        Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled);
        Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user);
        IApplicationUser GetCurrentUser(IUnitOfWork uow, IPrincipal principal);
    }
}