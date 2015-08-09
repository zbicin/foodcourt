using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodCourt.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult Test()
        {
            FoodCourt.Model.Group a = null;
            var f = a.Id != null;
            return View();
        }
    }
}