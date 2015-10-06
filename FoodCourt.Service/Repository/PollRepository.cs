using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using FoodCourt.Model;
using FoodCourt.Model.Identity;

namespace FoodCourt.Service.Repository
{
    public class PollRepository : BaseRepository<Poll>, IPollRepository
    {
        public PollRepository(IUnitOfWork unitOfWork, IApplicationUser currentUser) : base(unitOfWork, currentUser)
        {
        }

        public IQueryable<Poll> GetCurrentForGroup(Group group, string includes = "")
        {
            var query = this.GetAll(false, "Group").Where(p => p.Group.Id == group.Id && p.IsResolved == false);
            query = ResolveIncludes(includes, query);
            return query;
        }
    }
}