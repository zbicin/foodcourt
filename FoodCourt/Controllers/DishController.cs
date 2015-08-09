using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using FoodCourt.Controllers.Base;
using FoodCourt.Model;
using FoodCourt.ViewModel;

namespace FoodCourt.Controllers
{
    public class DishController : BaseApiController
    {
        public async Task<IHttpActionResult> GetList(Guid kindId, Guid restaurantId)
        {
            return await Search("", kindId, restaurantId);
        }

        public async Task<IHttpActionResult> Search(string searchPhrase, Guid kindId, Guid restaurantId)
        {
            if (kindId.ToString() == string.Empty)
            {
                throw new ArgumentNullException("kindId");
            }
            if (restaurantId.ToString() == string.Empty)
            {
                throw new ArgumentNullException("restaurantId");
            }

            var query =
                UnitOfWork.DishRepository.Search(searchPhrase, "Restaurant, Kind")
                    .Where(d => d.Restaurant.Id == restaurantId && d.Kind.Id == kindId);

            var viewModelQuery = query.Select(d => new DishViewModel()
            {
                Id = d.Id,
                KindId = d.Kind.Id,
                RestaurantId = d.Restaurant.Id,
                Name = d.Name
            });

            var viewModelList = await viewModelQuery.ToListAsync();
            return Ok(viewModelList);
        }

        public async Task<IHttpActionResult> Put(DishViewModel dish)
        {
            if (dish == null) throw new ArgumentNullException("dish");

            if (dish.RestaurantId.ToString() == string.Empty)
            {
                throw new InvalidOperationException("Could not create Dish without providing RestaurantId.");
            }
            if (dish.KindId.ToString() == string.Empty)
            {
                throw new InvalidOperationException("Could not create Dish without providing KindId.");
            }

            var existingDish = UnitOfWork.DishRepository.Search(dish.Name, "Restaurant,Kind", true).FirstOrDefault();

            if (existingDish != null)
            {
                return Conflict();
            }
            else
            {
                Dish newDish = new Dish()
                {
                    Restaurant = new Restaurant() { Id = dish.RestaurantId },
                    Kind = new Kind() { Id = dish.KindId },

                    Name = dish.Name
                };

                // attach restaurant & kind
                UnitOfWork.Attach(newDish.Restaurant);
                UnitOfWork.Attach(newDish.Kind);

                await UnitOfWork.DishRepository.Insert(newDish);

                return Ok(new DishViewModel()
                {
                    Id = newDish.Id,
                    Name = newDish.Name,
                    KindId = newDish.Kind.Id,
                    RestaurantId = newDish.Restaurant.Id
                });
            }

        }
    }
}