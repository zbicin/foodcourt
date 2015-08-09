using System;
using System.Collections.Generic;
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

        public OrderBasket()
        {
            MatchedOrders = new List<Order>();
            PossibleOrders = new List<Order>();
        }
    }
}