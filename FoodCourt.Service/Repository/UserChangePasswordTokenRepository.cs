using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodCourt.Model.Identity;

namespace FoodCourt.Service.Repository
{
    class UserChangePasswordTokenRepository : BaseRepository<UserChangePasswordToken>, IUserChangePasswordTokenRepository
    {
        public UserChangePasswordTokenRepository(IUnitOfWork unitOfWork, IApplicationUser currentUser) : base(unitOfWork, currentUser)
        {
        }

        public async Task<UserChangePasswordToken> GetForUser(ApplicationUser user)
        {
            return await GetForUser(user.Id);
        }

        public async Task<UserChangePasswordToken> GetForUser(Guid userId)
        {
            return await Database.Users.Where(u => u.Id == userId).Select(u => u.ChangePasswordToken).SingleAsync();
        }
    }
}
