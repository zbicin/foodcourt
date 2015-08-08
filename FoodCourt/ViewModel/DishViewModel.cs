using System;

namespace FoodCourt.ViewModel
{
    public class DishViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid KindId { get; set; }
        public Guid RestaurantId { get; set; }
    }
}