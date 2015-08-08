using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using FoodCourt.Controllers.Base;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using FoodCourt.Lib;
using FoodCourt.Model;
using FoodCourt.Model.Identity;
using FoodCourt.ViewModel;

namespace FoodCourt.Controllers
{
    public class OrderController : BaseApiController
    {
        public async Task<IHttpActionResult> GetListForPoll(Guid pollId)
        {
            var query = UnitOfWork.OrderRepository.GetForPoll(pollId);
            var viewModelQuery = query.Select(o => new OrderViewModel()
            {
                DishId = o.Dish.Id,
                Dish = o.Dish.Name,

                KindId = o.Dish.Kind.Id,
                Kind = o.Dish.Kind.Name,

                RestaurantId = o.Dish.Restaurant.Id,
                Restaurant = o.Dish.Restaurant.Name,

                IsOptional = o.IsOptional,
                IsHelpNeeded = o.IsHelpNeeded,

                UserEmail = o.User.Email
            });

            var viewModelList = await viewModelQuery.ToListAsync();

            return Ok(viewModelList);
        }

        public async Task<IHttpActionResult> Put(CreateOrderViewModel order)
        {
            Dish dish = await UnitOfWork.DishRepository.SingleOrDefault(order.DishId);
            if (dish == null)
            {
                return BadRequest("Wrong dish id.");
            }

            Poll currentPoll = await UnitOfWork.PollRepository.GetCurrentForGroup(CurrentGroup).FirstOrDefaultAsync();
            if (currentPoll == null)
            {
                return BadRequest("No active poll at this time.");
            }

            Order newOrder = new Order()
            {
                Dish = dish,
                IsOptional = order.IsOptional,
                IsHelpNeeded = order.IsHelpNeeded,
                User = (ApplicationUser) CurrentUser,
                Poll = currentPoll
            };

            await UnitOfWork.OrderRepository.Insert(newOrder);

            return Ok(new OrderViewModel()
            {
                DishId = newOrder.Dish.Id,
                Dish = newOrder.Dish.Name,

                KindId = newOrder.Dish.Kind.Id,
                Kind = newOrder.Dish.Kind.Name,

                RestaurantId = newOrder.Dish.Restaurant.Id,
                Restaurant = newOrder.Dish.Restaurant.Name,

                IsOptional = newOrder.IsOptional,
                IsHelpNeeded = newOrder.IsHelpNeeded,

                UserEmail = newOrder.User.Email
            });
        }
    }
}