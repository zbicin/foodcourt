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
        private IBaseRepository<Dish> _dishRepository;
        public IDishRepository DishRepository
        {
            get { return (IDishRepository)(_dishRepository ?? (_dishRepository = new BaseRepository<Dish>(this, CurrentUser))); }
        }

        private IBaseRepository<Group> _groupRepository;
        public IGroupRepository GroupRepository
        {
            get { return (IGroupRepository)(_groupRepository ?? (_groupRepository = new BaseRepository<Group>(this, CurrentUser))); }
        }

        private IBaseRepository<Kind> _kindRepository;
        public IKindRepository KindRepository
        {
            get { return (IKindRepository)(_kindRepository ?? (_kindRepository = new KindRepository(this, CurrentUser))); }
        }

        private IBaseRepository<Order> _orderRepository;
        public IOrderRepository OrderRepository
        {
            get { return (IOrderRepository)(_orderRepository ?? (_orderRepository = new BaseRepository<Order>(this, CurrentUser))); }
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

    public class KindRepository : BaseRepository<Kind>, IKindRepository
    {
        public KindRepository(UnitOfWork unitOfWork, IApplicationUser currentUser)
            : base(unitOfWork, currentUser)
        {
        }

        public IQueryable<Kind> Search(string searchPhrase, string includes = "")
        {
            return
                this.GetAll(false, includes)
                    .Where(k => k.Name.Contains(searchPhrase));
        }
    }
}
