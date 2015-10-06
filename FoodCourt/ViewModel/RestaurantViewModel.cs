using System;
using FoodCourt.Model;

namespace FoodCourt.ViewModel
{
    public class RestaurantViewModel
    {
        public RestaurantViewModel()
        {
        }

        public RestaurantViewModel(Model.Restaurant baseRestaurant)
        {
            Id = baseRestaurant.Id;
            MenuUrl = baseRestaurant.MenuUrl;
            Name = baseRestaurant.Name;
            PhoneNumber = baseRestaurant.PhoneNumber;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string MenuUrl { get; set; }

        public Restaurant UpdateModel(Model.Restaurant baseRestaurant)
        {
            baseRestaurant.MenuUrl = this.MenuUrl;
            baseRestaurant.Name = this.Name;
            baseRestaurant.PhoneNumber = this.PhoneNumber;
            return baseRestaurant;
        }
    }
}