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
            var list = await query.ToListAsync();

            var viewModelList = list.Select(r => new RestaurantViewModel(r));

            return Ok(viewModelList);
        }

        public async Task<IHttpActionResult> Put(RestaurantViewModel restaurant)
        {
            Restaurant newRestaurant = new Restaurant()
            {
                Group = CurrentGroup
            };
            restaurant.UpdateModel(newRestaurant);

            var existingRestaurant = UnitOfWork.RestaurantRepository.Search(restaurant.Name, "Group", true)
                .FirstOrDefault(r => r.Group.Id == CurrentGroup.Id);

            if (existingRestaurant != null)
            {
                return Conflict();
            }

            await UnitOfWork.RestaurantRepository.Insert(newRestaurant);

            return Ok(new RestaurantViewModel(newRestaurant));
        }

        
        public async Task<IHttpActionResult> Update(RestaurantViewModel restaurant)
        {
            var existingRestaurant = UnitOfWork.RestaurantRepository.Search(restaurant.Name, "Group", true)
                .FirstOrDefault(r => r.Group.Id == CurrentGroup.Id);

            if (existingRestaurant == null)
            {
                return NotFound();
            }

            restaurant.UpdateModel(existingRestaurant);

            await UnitOfWork.RestaurantRepository.Update(existingRestaurant);

            return Ok(new RestaurantViewModel(existingRestaurant));
        }
    }
}
