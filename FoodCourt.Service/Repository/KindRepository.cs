using System.Linq;
using FoodCourt.Model;
using FoodCourt.Model.Identity;

namespace FoodCourt.Service.Repository
{
    public class KindRepository : BaseRepository<Kind>, IKindRepository
    {
        public KindRepository(UnitOfWork unitOfWork, IApplicationUser currentUser)
            : base(unitOfWork, currentUser)
        {
        }

        public IQueryable<Kind> Search(string searchPhrase, string includes = "", bool useExplicitComparison = false)
        {
            if (string.IsNullOrWhiteSpace(searchPhrase))
            {
                return GetAll(false, includes);
            }

            return
                useExplicitComparison
                ? GetAll(false, includes)
                        .Where(k => k.Name == searchPhrase)
                : GetAll(false, includes)
                    .Where(k => k.Name.Contains(searchPhrase));
        }
    }
}