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
using FoodCourt.Service;
using FoodCourt.ViewModel;

namespace FoodCourt.Controllers
{
    public class OrderController : BaseApiController
    {
        [System.Web.Http.HttpGet]
        public async Task<IHttpActionResult> GetMatchesForPoll(Guid pollId)
        {
            var query = UnitOfWork.OrderRepository.GetForPoll(pollId, "Dish.Kind");
            List<Order> orders = query.ToList();

            OrderMatchHandler matchHandler = new OrderMatchHandler(orders);
            matchHandler.ReduceAmountOfBaskets();
            matchHandler.BalanceBaskets();

            var viewModelQuery = matchHandler.ReducedBaskets.Select(b => new MatchedOrderViewModel()
            {
                Orders = b.Orders.Select(o => new OrderViewModel()
                {
                    RestaurantId = b.RestaurantId,
                    Dish = o.Dish.Name,
                    DishId = o.Dish.Id,
                    Kind = o.Dish.Kind.Name,
                    KindId = o.Dish.Kind.Id,
                    IsHelpNeeded = o.IsHelpNeeded,
                    Restaurant = o.Dish.Restaurant.Name,
                    UserEmail = o.CreatedBy.Email
                }).ToList(),
                RestaurantId = b.RestaurantId
            });

            var viewModelList = viewModelQuery.ToList();

            return Ok(viewModelList);
        }


        public async Task<IHttpActionResult> Put(CreateOrderViewModel order)
        {
            Dish dish = await UnitOfWork.DishRepository.SingleOrDefault(order.DishId, false, "Restaurant,Kind");
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
                //IsOptional = order.IsOptional,
                IsHelpNeeded = order.IsHelpNeeded,
                Poll = currentPoll
            };

            // validate if user doesn't have other order from the same restaurant within the same poll
            Restaurant restaurant = dish.Restaurant;
            bool hasOtherOrderInRestaurant =
                await
                    UnitOfWork.OrderRepository.GetForPoll(currentPoll)
                        .AnyAsync(o => o.CreatedBy.Id == CurrentUser.Id && o.Dish.Restaurant.Id == restaurant.Id);

            if (!hasOtherOrderInRestaurant)
            {
                await UnitOfWork.OrderRepository.Insert(newOrder);

                return Ok(new OrderViewModel()
                {
                    DishId = newOrder.Dish.Id,
                    Dish = newOrder.Dish.Name,

                    KindId = newOrder.Dish.Kind.Id,
                    Kind = newOrder.Dish.Kind.Name,

                    RestaurantId = newOrder.Dish.Restaurant.Id,
                    Restaurant = newOrder.Dish.Restaurant.Name,

                    //IsOptional = newOrder.IsOptional,
                    IsHelpNeeded = newOrder.IsHelpNeeded,

                    UserEmail = newOrder.CreatedBy.Email
                });
            }
            else
            {
                return BadRequest("Current user already have ordered in this restaurant.");
            }
        }
    }
}