using System;
using System.Linq;
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
        IGroupRepository GroupRepository { get; }
        IKindRepository KindRepository { get; }
        IOrderRepository OrderRepository { get; }
        IPollRepository PollRepository { get; }
        IRestaurantRepository RestaurantRepository { get; }
        void SetCurrentUser(IApplicationUser currentUser);
        void Dispose();
        void Attach<T>(T entity) where T : BaseEntity;
    }

    public interface IDishRepository : IBaseRepository<Dish>
    {
    }

    public interface IGroupRepository : IBaseRepository<Group>
    {
    }

    public interface IKindRepository : IBaseRepository<Kind>
    {
        IQueryable<Kind> Search(string searchPhrase, string includes = "");
    }

    public interface IOrderRepository : IBaseRepository<Order>
    {
    }
}