using System;
using System;
using System.Collections.Generic;
using FoodCourt.ViewModel;

namespace FoodCourt.ViewModel
{
    public class MatchedOrderViewModel
    {
        public List<OrderViewModel> Orders { get; set; }
        public Guid RestaurantId { get; set; }
    }
}