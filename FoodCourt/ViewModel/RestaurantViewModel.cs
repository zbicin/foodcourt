using System;

namespace FoodCourt.ViewModel
{
    public class RestaurantViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string MenuUrl { get; set; }
    }
}