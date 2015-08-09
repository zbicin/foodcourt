using System;
using System.Threading.Tasks;
using FoodCourt.Model.Identity;

namespace FoodCourt.Service.Repository
{
    public interface IUserChangePasswordTokenRepository : IBaseRepository<UserChangePasswordToken>
    {
        Task<UserChangePasswordToken> GetForUser(ApplicationUser user);
        Task<UserChangePasswordToken> GetForUser(Guid userId);
    }
}