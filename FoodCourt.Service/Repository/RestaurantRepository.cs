using System.Linq;
using FoodCourt.Model;
using FoodCourt.Model.Identity;

namespace FoodCourt.Service.Repository
{
    public class RestaurantRepository : BaseRepository<Restaurant>, IRestaurantRepository
    {
        public RestaurantRepository(IUnitOfWork unitOfWork, IApplicationUser currentUser) : base(unitOfWork, currentUser)
        {
        }

        public IQueryable<Restaurant> Search(string searchPhrase, string includes = "")
        {
            if (string.IsNullOrWhiteSpace(searchPhrase))
            {
                return GetAll(false, includes);
            }

            return
                this.GetAll(false, includes)
                    .Where(r => r.Name.Contains(searchPhrase) || r.PhoneNumber.Contains(searchPhrase));
        }
    }
}