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

    public interface IDishRepository : IBaseRepository<Dish>
    {
        IQueryable<Dish> Search(string searchPhrase, string includes = "");
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
        IQueryable<Order> GetForPoll(Poll poll, string includes ="");
        IQueryable<Order> GetForPoll(Guid pollId, string includes = "");
    }
}