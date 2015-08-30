using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FoodCourt.Model;
using FoodCourt.Model.Identity;
using FoodCourt.Service;
using FoodCourt.ViewModel;
using System.Net;
using System.Web.Routing;
using Newtonsoft.Json;

namespace FoodCourt.Controllers
{
    public class DebugController : Controller
    {

        public ActionResult AlgorithmTester(int? kindsMaxCount, int? restaurantsMaxCount, int? dishesMaxCount, int? usersMaxCount, int? maxOrdersPerUser)
        {
            if (!kindsMaxCount.HasValue || !restaurantsMaxCount.HasValue || !dishesMaxCount.HasValue || !usersMaxCount.HasValue || !maxOrdersPerUser.HasValue)
            {
                return Redirect("/Debug/AlgorithmTester/?kindsMaxCount=10&restaurantsMaxCount=50&dishesMaxCount=100&usersMaxCount=60&maxOrdersPerUser=4");
            }

            var random = new Random();
            var group = new Group()
            {
                Id = Guid.NewGuid(),
                Name = "Guinea Pigs"
            };
            var users = GenerateRandomUsers(group, random, usersMaxCount.Value);
            var restaurants = GenerateRestaurants(group, random, restaurantsMaxCount.Value);
            var kinds = GenerateKinds(group, random, kindsMaxCount.Value);
            var dishes = GenerateDishes(group, restaurants, kinds, random, dishesMaxCount.Value);
            var randomOrders = GenerateRandomOrders(random, users, dishes, maxOrdersPerUser.Value);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var matches = new OrderMatchHandler(randomOrders).ProcessOrders();
            stopwatch.Stop();

            ViewBag.Time = stopwatch.Elapsed;
            ViewBag.Orders = randomOrders.Select(o => new OrderViewModel()
            {
                Dish = o.Dish.Name,
                DishId = o.Dish.Id,
                Kind = o.Dish.Kind.Name,
                KindId = o.Dish.Kind.Id,
                Id = o.Id,
                RestaurantId = o.Dish.Restaurant.Id,
                Restaurant = o.Dish.Restaurant.Name,
                IsHelpNeeded = o.IsHelpNeeded,
                IsOptional = false,
                UserEmail = o.CreatedBy.Email
            }).OrderBy(o => o.UserEmail).ToList();

            ViewBag.Matches = matches.Select(m => new MatchedOrderViewModel()
            {
                Captain = m.Captain.Email,
                RestaurantId = m.RestaurantId,
                Orders = m.MatchedOrders.Select(o => new OrderViewModel()
                {
                    Dish = o.Dish.Name,
                    DishId = o.Dish.Id,
                    Kind = o.Dish.Kind.Name,
                    KindId = o.Dish.Kind.Id,
                    Id = o.Id,
                    RestaurantId = o.Dish.Restaurant.Id,
                    Restaurant = o.Dish.Restaurant.Name,
                    IsHelpNeeded = o.IsHelpNeeded,
                    IsOptional = false,
                    UserEmail = o.CreatedBy.Email
                }).ToList()
            }).OrderBy(m => m.RestaurantId).ToList();

            return View();
        }

        private List<Order> GenerateRandomOrders(Random random, List<ApplicationUser> users, List<Dish> dishes, int maxOrdersPerUser)
        {
            var result = new List<Order>();
            foreach (var user in users)
            {
                var ordersCount = random.Next(1, maxOrdersPerUser+1);
                for (var i = 0; i < ordersCount; i++)
                {
                    var hasUserOrderedAnotherDishInThisRestaurant = false;
                    int doIterations = 0; // cutout
                    Order newOrder;
                    do
                    {
                        newOrder = new Order()
                        {
                            Id = Guid.NewGuid(),
                            Dish = dishes[random.Next(0, dishes.Count)],
                            CreatedBy = users[random.Next(0, users.Count)],
                            IsHelpNeeded = random.Next(0, 2) == 1
                        };

                        hasUserOrderedAnotherDishInThisRestaurant = result.Any(o =>
                            o.Dish.Restaurant.Id == newOrder.Dish.Restaurant.Id
                            && o.CreatedBy.Id == newOrder.CreatedBy.Id
                            );
                        doIterations++;
                    } while (hasUserOrderedAnotherDishInThisRestaurant && doIterations < 1000);

                    if (doIterations < 1000)
                    {
                        result.Add(newOrder);                        
                    }
                }
            }
            return result;
        }

        private List<Dish> GenerateDishes(Group group, List<Restaurant> restaurants, List<Kind> kinds, Random random, int dishesMaxCount)
        {
            var result = new List<Dish>();
            var dishesCount = random.Next(1, dishesMaxCount + 1);
            for (var i = 0; i < dishesCount; i++)
            {
                result.Add(new Dish()
                {
                    Id = Guid.NewGuid(),
                    Group = group,
                    Name = String.Format("Dish #{0}", i),
                    Kind = kinds[random.Next(0, kinds.Count)],
                    Restaurant = restaurants[random.Next(0, restaurants.Count)]
                });
            }
            return result;
        }

        private List<Kind> GenerateKinds(Group group, Random random, int kindsMaxCount)
        {
            var result = new List<Kind>();
            var kindsCount = random.Next(1, kindsMaxCount + 1);
            for (var i = 0; i < kindsCount; i++)
            {
                result.Add(new Kind()
                {
                    Id = Guid.NewGuid(),
                    Group = group,
                    Name = String.Format("Kind #{0}", i)
                });
            }
            return result;
        }

        private List<Restaurant> GenerateRestaurants(Group group, Random random, int restaurantsMaxCount)
        {
            var result = new List<Restaurant>();
            var restaurantsCount = random.Next(1, restaurantsMaxCount + 1);
            for (var i = 0; i < restaurantsCount; i++)
            {
                result.Add(new Restaurant()
                {
                    Id = Guid.NewGuid(),
                    Group = group,
                    Name = String.Format("Restaurant #{0}", i)
                });
            }
            return result;
        }

        private List<ApplicationUser> GenerateRandomUsers(Group group, Random random, int usersMaxCount)
        {
            using (var webClient = new WebClient())
            {
                var result = new List<ApplicationUser>();
                var usersCount = random.Next(2, usersMaxCount + 1);
                var randomuserMeResponse = JsonConvert.DeserializeObject<RandomuserMeViewModels.RandomuserMeResponse>(webClient.DownloadString(String.Format("https://randomuser.me/api/?results={0}", usersCount)));

                return randomuserMeResponse.Results.Select(r => new ApplicationUser()
                {
                    Id = Guid.NewGuid(),
                    UserName = r.User.Email,
                    Email = r.User.Email,
                    Group = group
                }).ToList();
            }
        }


    }
}