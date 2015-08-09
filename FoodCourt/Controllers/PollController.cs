﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using FoodCourt.Controllers.Base;
using FoodCourt.Lib;
using FoodCourt.Model;
using FoodCourt.ViewModel;

namespace FoodCourt.Controllers
{
    [AuthorizeRedirectToRegister]
    public class PollController : BaseController
    {

        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Tries to get current active poll based on current user's group.
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> TryGetCurrentPoll()
        {
            var viewModelQuery = GetViewModelQuery(UnitOfWork.PollRepository.GetCurrentForGroup(CurrentGroup, "Orders.Dish.Kind, Orders.Dish.Restaurant"));
            PollViewModel currentPollViewModel = await viewModelQuery.FirstOrDefaultAsync();

            if (currentPollViewModel == null)
            {
                // create new poll
                Poll newPoll = new Poll()
                {
                    Group = CurrentGroup
                };

                await UnitOfWork.PollRepository.Insert(newPoll);

                // only if there is no active poll at this point of time
                currentPollViewModel = new PollViewModel
                {
                    Group = CurrentGroup.Name,
                    IsFinished = false,
                    IsResolved = false,
                    Id = newPoll.Id
                };
            }

            return Json(currentPollViewModel, JsonRequestBehavior.AllowGet);
        }

        private IQueryable<PollViewModel> GetViewModelQuery(IQueryable<Poll> query)
        {
            return query.Select(p => new PollViewModel
            {
                Id = p.Id,
                Group = p.Group.Name,

                Remarks = p.Remarks,
                Orders = p.Orders.Select(o => new OrderViewModel()
                {
                    DishId = o.Dish.Id,
                    Dish = o.Dish.Name,

                    KindId = o.Dish.Kind.Id,
                    Kind = o.Dish.Kind.Name,

                    RestaurantId = o.Dish.Restaurant.Id,
                    Restaurant = o.Dish.Restaurant.Name,

                    //IsOptional = o.IsOptional,
                    IsHelpNeeded = o.IsHelpNeeded,

                    UserEmail = o.CreatedBy.Email
                }),

                ETA = p.ETA,

                IsFinished = p.IsFinished,
                FinishedAt = p.FinishedAt,

                IsResolved = p.IsResolved
            });
        }
    }
}
