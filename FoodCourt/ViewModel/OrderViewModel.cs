using System;

namespace FoodCourt.ViewModel
{
    public class OrderViewModel
    {
        public Guid DishId { get; set; }
        public string Dish { get; set; }

        public Guid KindId { get; set; }
        public string Kind { get; set; }

        public Guid RestaurantId { get; set; }
        public string Restaurant { get; set; }

        public bool IsOptional { get; set; }
        public bool IsHelpNeeded { get; set; }
        public string UserEmail { get; set; }
    }
}