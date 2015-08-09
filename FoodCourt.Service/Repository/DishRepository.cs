using System.Linq;
using FoodCourt.Model;
using FoodCourt.Model.Identity;

namespace FoodCourt.Service.Repository
{
    public class DishRepository : BaseRepository<Dish>, IDishRepository
    {
        public DishRepository(IUnitOfWork unitOfWork, IApplicationUser currentUser) : base(unitOfWork, currentUser)
        {
        }

        public IQueryable<Dish> Search(string searchPhrase, string includes = "", bool useExplicitComparison = false)
        {
            if (string.IsNullOrWhiteSpace(searchPhrase))
            {
                return GetAll(false, includes);
            }

            return
                useExplicitComparison 
                ? this.GetAll(false, includes)
                        .Where(k => k.Name == searchPhrase)
                :this.GetAll(false, includes)
                    .Where(k => k.Name.Contains(searchPhrase));
        }
    }
}