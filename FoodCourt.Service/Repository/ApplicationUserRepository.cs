using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using FoodCourt.Model;
using FoodCourt.Model.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FoodCourt.Service.Repository
{
    public class ApplicationUserRepository : UserStore<ApplicationUser, Role, Guid, UserLogin, UserRole, UserClaim>, IApplicationUserRepository
    {

        public ApplicationUserRepository(IUnitOfWork uow)
            : base(uow.Db)
        {
            if (uow == null)
            {
                throw new ArgumentNullException("unitOfWork must be provided to the repository");
            }
            _unitOfWork = uow;
            DbSet = (DbSet<ApplicationUser>)Database.Users;
            //_currentUser = GetSystemUser();
        }

        protected static ApplicationUserRepository Self;
        public IApplicationUser GetCurrentUser(IUnitOfWork uow, IPrincipal principal)
        {
            IIdentity identity = principal.Identity;
            if (identity.IsAuthenticated)
            {
                if (Self == null)
                {
                    Self = new ApplicationUserRepository(uow);
                }
                else
                {
                    return Self.CurrentUser;
                }

                string userName = identity.Name;
                try
                {
                    ApplicationUser systemUser = Self.DbSet.Single(u => u.UserName == userName);
                    Self.CurrentUser = systemUser;

                    return Self.CurrentUser;
                }
                catch (Exception e)
                {
                    // could not retrieve current user (why?)
                    Self = null;
                    return null;
                }

            }

            return null;
        }

        public ApplicationUser GetSystemUser()
        {
            try
            {
                if (SystemUser == null)
                {
                    SystemUser = DbSet.Single(u => u.UserName == "SYSTEM");
                }
            }
            catch (Exception e)
            {
                throw new RecordNotFoundException("Could not retrieve SYSTEM user.", e);
            }

            return SystemUser;
        }

        #region IBaseRepository implementation
        private readonly IUnitOfWork _unitOfWork;
        private IApplicationUser _currentUser;

        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        protected IApplicationUser CurrentUser
        {
            get { return _currentUser; }
            set { _currentUser = value; }
        }

        internal DbSet<ApplicationUser> DbSet;
        protected ApplicationUser SystemUser;
        internal ApplicationDbContext Database { get { return _unitOfWork.Db; } }


        public async Task<ApplicationUser> Single(object primaryKey, bool withDeleted = false)
        {
            var entity = await DbSet.FindAsync(primaryKey);
            if (withDeleted == false && entity.IsDeleted)
            {
                return null;
            }
            return entity;
        }

        protected IQueryable<ApplicationUser> ResolveIncludes(string include, IQueryable<ApplicationUser> query)
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
        public IQueryable<ApplicationUser> GetAll(bool withDeleted = false, string includes = "")
        {
            IQueryable<ApplicationUser> set = DbSet;
            if (withDeleted == false)
            {
                set = set.Where(o => o.IsDeleted == false);
            }

            set = ResolveIncludes(includes, set);

            return set;
        }

        public async Task<bool> Exists(object primaryKey, bool withDeleted = false)
        {
            return await Single(primaryKey, withDeleted) != null;
        }

        public async Task<int> Insert(ApplicationUser entity)
        {
            DbSet.Add(entity);
            return await Database.SaveChangesAsync();
        }

        public async Task<int> Update(ApplicationUser entity)
        {
            DbSet.Attach(entity);
            Database.Entry(entity).State = EntityState.Modified;
            return await Database.SaveChangesAsync();
        }

        public async Task<int> Delete(ApplicationUser entity)
        {
            if (Database.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;

            return await Database.SaveChangesAsync();
        }
    }

        #endregion
}
