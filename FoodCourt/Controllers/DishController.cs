using System;
using System.Collections.Generic;
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
        public async Task<IHttpActionResult> Put(DishViewModel dish)
        {
            if (dish == null) throw new ArgumentNullException("dish");

            if (dish.RestaurantId.ToString() == "")
            {
                throw new InvalidOperationException("Could not create Dish without providing RestaurantId.");
            }
            if (dish.KindId.ToString() == "")
            {
                throw new InvalidOperationException("Could not create Dish without providing KindId.");
            }

            Dish newDish = new Dish()
            {
                Restaurant = new Restaurant() {Id = dish.RestaurantId},
                Kind = new Kind() {Id = dish.KindId},

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