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
        IBaseRepository<Order> OrderRepository { get; }
        void SetCurrentUser(IApplicationUser currentUser);
    }
}