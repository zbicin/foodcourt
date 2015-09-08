using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using FoodCourt.Model;
using FoodCourt.Model.Identity;

namespace FoodCourt.Service.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly IUnitOfWork _unitOfWork;
        private IApplicationUser _currentUser;
        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        public IApplicationUser CurrentUser
        {
            get { return _currentUser; }
            set { _currentUser = value; }
        }

        internal DbSet<T> DbSet;
        internal ApplicationDbContext Database { get { return _unitOfWork.Db; } }

        public BaseRepository(IUnitOfWork unitOfWork, IApplicationUser currentUser)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }
            if (currentUser == null)
            {
                throw new ArgumentNullException("currentUser");
            }

            _unitOfWork = unitOfWork;
            _currentUser = currentUser;

            DbSet = Database.Set<T>();
        }

        protected IQueryable<T> ResolveIncludes(string include, IQueryable<T> query)
        {
            if (string.IsNullOrEmpty(include) == false)
            {
                var includes = include.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string includeProperty in includes)
                {
                    query = query.Include(includeProperty.Trim());
                }
            }
            return query;
        }

        public async Task<T> Single(Guid primaryKey, bool withDeleted = false, string include = "")
        {
            T entity;
            var query = ResolveIncludes(include, DbSet);

            try
            {
                entity = await query.FirstAsync(e => e.Id == primaryKey);
            }
            catch (InvalidOperationException e)
            {
                throw new RecordNotFoundException(string.Format("Record of type {0} with ID {1} could not be found", typeof(T).Name, primaryKey.ToString(), e));
            }

            if (withDeleted == false && entity.IsDeleted)
            {
                return null;
            }

            return entity;
        }

        public async Task<T> SingleOrDefault(Guid primaryKey, bool withDeleted = false, string include = "")
        {
            var entity = await ResolveIncludes(include, DbSet).FirstOrDefaultAsync(e => e.Id == primaryKey);
            if (withDeleted == false && entity.IsDeleted)
            {
                return null;
            }

            return entity;
        }

        public IQueryable<T> GetAll(bool withDeleted = false, string include = "")
        {
            IQueryable<T> set = ResolveIncludes(include, DbSet);

            if (withDeleted == false)
            {
                set = set.Where(o => o.IsDeleted == false);
            }

            return set;
        }

        public async Task<bool> Exists(Guid primaryKey, bool withDeleted = false)
        {
            return await SingleOrDefault(primaryKey, withDeleted) != null;
        }

        public async Task<int> Insert(T entity)
        {
            Database.Entry(entity).State = EntityState.Added;

            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = (ApplicationUser)_currentUser;

            DbSet.Add(entity);

            return await Database.SaveChangesAsync();
        }

        public async Task<int> Update(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = (ApplicationUser)_currentUser;

            //Database.Entry(entity).State = EntityState.Modified;

            return await Database.SaveChangesAsync();
        }

        public async Task<int> Delete(T entity)
        {
            if (Database.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = (ApplicationUser)_currentUser;

            return await Database.SaveChangesAsync();
        }
    }
}