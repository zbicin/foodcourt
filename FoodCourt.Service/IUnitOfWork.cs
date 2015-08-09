using System;
using FoodCourt.Model;
using FoodCourt.Model.Identity;
using FoodCourt.Service.Repository;

namespace FoodCourt.Service
{
    public interface IUnitOfWork : IDisposable
    {
        ApplicationDbContext Db { get; }
        IApplicationUser CurrentUser { get; }
        IApplicationUserRepository UserAccountRepository { get; }
        IDishRepository DishRepository { get; }
        IBaseRepository<Group> GroupRepository { get; }
        IKindRepository KindRepository { get; }
        IOrderRepository OrderRepository { get; }
        IPollRepository PollRepository { get; }
        IRestaurantRepository RestaurantRepository { get; }
        IUserChangePasswordTokenRepository UserChangePasswordTokenRepository { get; }
        void SetCurrentUser(IApplicationUser currentUser);
        void Dispose();
        void Attach<T>(T entity) where T : BaseEntity;
    }
}