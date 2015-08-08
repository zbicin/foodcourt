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
        public async Task<IHttpActionResult> GetList()
        {
            return await Search("");
        }

        public async Task<IHttpActionResult> Search(string searchPhrase)
        {
            var query = UnitOfWork.RestaurantRepository.Search(searchPhrase);
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
                Name = restaurant.Name,
                PhoneNumber = restaurant.PhoneNumber
            };

            await UnitOfWork.RestaurantRepository.Insert(newRestaurant);

            return Ok(new RestaurantViewModel()
            {
                Id = newRestaurant.Id,
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
