using System.Linq;
using System.Threading.Tasks;
using FoodCourt.Model;

namespace FoodCourt.Service.Repository
{
    public interface IPollRepository : IBaseRepository<Poll>
    {
        IQueryable<Poll> GetCurrentForGroup(Group group, string includes = "");
    }
}