using FoodCourt.Model.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using FoodCourt.ViewModel;

namespace FoodCourt.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Order");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LandingViewModel viewModel)
        {
            // TODO: Krzysiek - refactor Index
            // create account
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = viewModel.AdminEmail, Email = viewModel.AdminEmail };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    return RedirectToAction("ChangePassword", "Manage");
                }
                // TODO: Krzysiek - show feedback with errors
                //AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(viewModel);  
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}