using System;
using System.Collections.Generic;
using System.Linq;
using FoodCourt.Model;
using FoodCourt.Model.Identity;

namespace FoodCourt.Service
{
    public class OrderBasket
    {
        /// <summary>
        /// How harmful removing this basket will be.
        /// Authors of all matched orders in this basket:
        /// 1.0 - have placed orders in other baskets as well, we can get rid of this basket
        /// 0.0 - haven't placed an order in any other basket, we can't get rid of this basket
        /// (0.0, 1.0) - somewhere between
        /// </summary>
        public double AbilityToGetRidOf { get; set; }
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

        public override string ToString()
        {
            return String.Format("{0} ({1} matched, {2} possible)", this.RestaurantName, this.MatchedOrders.Count, this.PossibleOrders.Count);
        }
    }
}