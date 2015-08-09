using System;
using System.Linq;
using System.Text;
using FoodCourt.Model;
using FoodCourt.Model.Identity;
using FoodCourt.Service.Repository;

namespace FoodCourt.Service
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationDbContext Db
        {
            get { return _context; }
        }

        public IApplicationUser CurrentUser { get; private set; }
        public void SetCurrentUser(IApplicationUser currentUser)
        {
            CurrentUser = currentUser;
        }

        public void Attach<T>(T entity) where T : BaseEntity
        {
            Db.Set<T>().Attach(entity);
        }

        #region Repositories
        private IApplicationUserRepository _applicationUserRepository;
        public IApplicationUserRepository UserAccountRepository
        {
            get
            {
                if (_applicationUserRepository == null)
                {
                    _applicationUserRepository = new ApplicationUserRepository(this);
                }
                return _applicationUserRepository;
            }
        }
        private IDishRepository _dishRepository;
        public IDishRepository DishRepository
        {
            get { return (_dishRepository ?? (_dishRepository = new DishRepository(this, CurrentUser))); }
        }

        private IBaseRepository<Group> _groupRepository;
        public IBaseRepository<Group> GroupRepository
        {
            get { return (_groupRepository ?? (_groupRepository = new BaseRepository<Group>(this, CurrentUser))); }
        }

        private IKindRepository _kindRepository;
        public IKindRepository KindRepository
        {
            get { return (_kindRepository ?? (_kindRepository = new KindRepository(this, CurrentUser))); }
        }

        private IOrderRepository _orderRepository;
        public IOrderRepository OrderRepository
        {
            get { return (IOrderRepository)(_orderRepository ?? (_orderRepository = new OrderRepository(this, CurrentUser))); }
        }

        private IPollRepository _pollRepository;
        public IPollRepository PollRepository
        {
            get { return _pollRepository ?? (_pollRepository = new PollRepository(this, CurrentUser)); }
        }

        private IRestaurantRepository _restaurantRepository;
        public IRestaurantRepository RestaurantRepository
        {
            get { return _restaurantRepository ?? (_restaurantRepository = new RestaurantRepository(this, CurrentUser)); }
        }
        #endregion
        #region IDisposable
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(IUnitOfWork unitOfWork, IApplicationUser currentUser) : base(unitOfWork, currentUser)
        {
        }

        public IQueryable<Order> GetForPoll(Poll poll, string includes = "")
        {
            return GetForPoll(poll.Id, includes);
        }

        public IQueryable<Order> GetForPoll(Guid pollId, string includes = "")
        {
            var query = this.GetAll(false, "CreatedBy,Dish.Restaurant").Where(o => o.Poll.Id == pollId);
            query = this.ResolveIncludes(includes, query);
            return query;
        }
    }
}
