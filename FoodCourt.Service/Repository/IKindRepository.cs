using System.Linq;
using FoodCourt.Model;

namespace FoodCourt.Service.Repository
{
    public interface IKindRepository : IBaseRepository<Kind>
    {
        IQueryable<Kind> Search(string searchPhrase, string includes = "", bool useExplicitComparison = false);
    }
}