using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using FoodCourt.Controllers.Base;
using FoodCourt.Model;
using FoodCourt.Service;
using FoodCourt.ViewModel;

namespace FoodCourt.Controllers
{
    public class RestaurantController : BaseApiController
    {
        public async Task<IHttpActionResult> GetList(Guid kindId)
        {
            return await Search("", kindId);
        }

        public async Task<IHttpActionResult> Search(string searchPhrase, Guid kindId)
        {
            var restaurantIds =
                UnitOfWork.DishRepository.GetAll(false, "Restaurant, Kind")
                    .Where(d => d.Kind.Id == kindId)
                    .Select(d => d.Restaurant.Id);
            var query = UnitOfWork.RestaurantRepository.Search(searchPhrase).Where(r => restaurantIds.Contains(r.Id));
            var viewModelQuery = query.Select(r => new RestaurantViewModel()
            {
                Id = r.Id,
                Name = r.Name,
                PhoneNumber = r.PhoneNumber,
                MenuUrl = r.MenuUrl
            });

            var viewModelList = await viewModelQuery.ToListAsync();

            return Ok(viewModelList);
        }

        public async Task<IHttpActionResult> Put(RestaurantViewModel restaurant)
        {
            Restaurant newRestaurant = new Restaurant()
            {
                Group = CurrentGroup,
                MenuUrl = restaurant.MenuUrl,
                Name = restaurant.Name,
                PhoneNumber = restaurant.PhoneNumber
            };

            await UnitOfWork.RestaurantRepository.Insert(newRestaurant);

            return Ok(new RestaurantViewModel()
            {
                Id = newRestaurant.Id,
                MenuUrl = newRestaurant.MenuUrl,
                Name = newRestaurant.Name,
                PhoneNumber = newRestaurant.PhoneNumber
            });
        }

        public async Task<IHttpActionResult> Post(RestaurantViewModel restaurant)
        {
            throw new NotImplementedException();
        }
    }
}
