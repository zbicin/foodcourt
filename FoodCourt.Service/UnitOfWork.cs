using System;
using System.Collections.Generic;
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

        #region Repositories
        private IBaseRepository<Dish> _dishRepository;
        public IBaseRepository<Dish> DishRepository
        {
            get { return _dishRepository ?? (_dishRepository = new BaseRepository<Dish>(this, CurrentUser)); }
        }

        private IBaseRepository<Group> _groupRepository;
        public IBaseRepository<Group> GroupRepository
        {
            get { return _groupRepository ?? (_groupRepository = new BaseRepository<Group>(this, CurrentUser)); }
        }

        private IBaseRepository<Kind> _kindRepository;
        public IBaseRepository<Kind> KindRepository
        {
            get { return _kindRepository ?? (_kindRepository = new BaseRepository<Kind>(this, CurrentUser)); }
        }

        private IBaseRepository<Order> _orderRepository;
        public IBaseRepository<Order> OrderRepository
        {
            get { return _orderRepository ?? (_orderRepository = new BaseRepository<Order>(this, CurrentUser)); }
        }

        private IBaseRepository<Poll> _pollRepository;
        public IBaseRepository<Poll> PollRepository
        {
            get { return _pollRepository ?? (_pollRepository = new BaseRepository<Poll>(this, CurrentUser)); }
        }

        private IBaseRepository<Restaurant> _restaurantRepository;
        public IBaseRepository<Restaurant> RestaurantRepository
        {
            get { return _restaurantRepository ?? (_restaurantRepository = new BaseRepository<Restaurant>(this, CurrentUser)); }
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
}
