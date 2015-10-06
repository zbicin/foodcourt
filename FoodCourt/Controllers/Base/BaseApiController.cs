using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using FoodCourt.Model;
using FoodCourt.Model.Identity;
using FoodCourt.Service;
using Microsoft.AspNet.Identity;


namespace FoodCourt.Controllers.Base
{
    public abstract class BaseApiController : ApiController, IDisposable
    {
        private IApplicationUser _currentUser;
        private IUnitOfWork _uow;

        protected IApplicationUser CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    if (User.Identity.IsAuthenticated == false)
                    {
                        throw new UnauthorizedAccessException("Not authenticated.");
                    }

                    Guid currentUserId = new Guid(User.Identity.GetUserId());

                    try
                    {

                        _currentUser =
                            UnitOfWork.UserAccountRepository.GetAll(false, "Group,Group.CreatedBy").Single(u => u.Id == currentUserId);
                        UnitOfWork.SetCurrentUser(_currentUser);
                    }
                    catch (InvalidOperationException exception)
                    {
                        // if invalid operation exception is thrown that means that user with Id form User.Identity does not
                        // longer exist in database (perhaps database was rebuilt?)
                        //throw new UnauthorizedAccessException("User not found.");
                        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                        {
                            Content = new StringContent("User not found"),
                            ReasonPhrase = "Unauthorized",
                            StatusCode = HttpStatusCode.Unauthorized
                        });
                    }
                }
                return _currentUser;
            }
        }

        protected Group CurrentGroup
        {
            get { return CurrentUser.Group; }
        }

        protected IUnitOfWork UnitOfWork
        {
            get
            {
                if (_uow == null)
                {
                    _uow = new UnitOfWork(new ApplicationDbContext());
                    _uow.SetCurrentUser(CurrentUser);
                }

                return _uow;
            }
        }

        private bool _disposed = false;

        protected override void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    UnitOfWork.Dispose();
                }
            }
            this._disposed = true;
        }

        protected new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}