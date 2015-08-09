using System;
using System.Linq;
using FoodCourt.Model;

namespace FoodCourt.Service.Repository
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        IQueryable<Order> GetForPoll(Poll poll, string includes ="");
        IQueryable<Order> GetForPoll(Guid pollId, string includes = "");
    }
}