using System.Linq;
using FoodCourt.Model;

namespace FoodCourt.Service.Repository
{
    public interface IRestaurantRepository : IBaseRepository<Restaurant>
    {
        IQueryable<Restaurant> Search(string searchPhrase, string includes = "", bool useExplicitComparison = false);
    }
}