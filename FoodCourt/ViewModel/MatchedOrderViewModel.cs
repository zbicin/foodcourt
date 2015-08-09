using System;
using System.Collections.Generic;

namespace FoodCourt.ViewModel
{
    public class MatchedOrderViewModel
    {
        public List<OrderViewModel> Orders { get; set; }
        public Guid RestaurantId { get; set; }
    }
}