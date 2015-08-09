using System;
using System.Collections.Generic;
using System.Linq;
using FoodCourt.Model;
using FoodCourt.Model.Identity;

namespace FoodCourt.Service
{
    public class OrderBasket
    {
        public Guid RestaurantId;
        public List<Order> MatchedOrders;
        public List<Order> PossibleOrders;

        public bool IsNotMatched = false;

        public ApplicationUser Captain;

        public string RestaurantName
        {
            get
            {
                if (MatchedOrders.Any())
                {
                    return MatchedOrders.First().Dish.Restaurant.Name;
                }

                return string.Empty;
            }
        }

        public OrderBasket()
        {
            MatchedOrders = new List<Order>();
            PossibleOrders = new List<Order>();
        }
    }
}