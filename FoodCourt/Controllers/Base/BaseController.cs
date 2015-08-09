using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using FoodCourt.Model;
using FoodCourt.Model.Identity;
using FoodCourt.Service;
using Microsoft.AspNet.Identity;

namespace FoodCourt.Controllers.Base
{
    public abstract class BaseController : Controller
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
                        //throw new UnauthorizedAccessException("Not authenticated.");
                        return new ApplicationUser();
                    }

                    Guid currentUserId = new Guid(User.Identity.GetUserId());

                    try
                    {

                        _currentUser = 
                            UnitOfWork.UserAccountRepository.GetAll(false, "Group").Single(u => u.Id == currentUserId);
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
                    if (!_disposing)
                    {
                        _uow.SetCurrentUser(CurrentUser);
                    }
                }

                return _uow;
            }
        }

        protected Postman Postman
        {
            get
            {
                if (_postman == null)
                {
                    _postman = new Postman(Server.MapPath("~/Views/EmailTemplate"));
                }
                return _postman;
            }
        }

        private bool _disposed = false;
        private bool _disposing = false;
        private Postman _postman;

        protected override void Dispose(bool disposing)
        {
            _disposing = disposing;
            if (!this._disposed)
            {
                if (disposing)
                {
                    UnitOfWork.Dispose();
                }
            }
            this._disposed = true;
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}