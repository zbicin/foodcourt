using System;

namespace FoodCourt.ViewModel
{
    public class CreateOrderViewModel
    {
        public Guid DishId { get; set; }
        public bool IsOptional { get; set; }
        public bool IsHelpNeeded { get; set; }
    }
}