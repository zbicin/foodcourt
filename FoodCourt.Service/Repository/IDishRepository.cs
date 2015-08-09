using System.Linq;
using FoodCourt.Model;

namespace FoodCourt.Service.Repository
{
    public interface IDishRepository : IBaseRepository<Dish>
    {
        IQueryable<Dish> Search(string searchPhrase, string includes = "", bool useExplicitComparison = false);
    }
}