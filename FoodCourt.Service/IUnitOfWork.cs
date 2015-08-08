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
        IBaseRepository<Dish> DishRepository { get; }
        IBaseRepository<Group> GroupRepository { get; }
        IBaseRepository<Kind> KindRepository { get; }
        IBaseRepository<Order> OrderRepository { get; }
        IPollRepository PollRepository { get; }
        IBaseRepository<Restaurant> RestaurantRepository { get; }
        void SetCurrentUser(IApplicationUser currentUser);
        void Dispose();
    }
}