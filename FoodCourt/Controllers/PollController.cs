﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
using FoodCourt.Controllers.Base;
using FoodCourt.Lib;
using FoodCourt.Model;
using FoodCourt.Model.Identity;
using FoodCourt.Service;
using FoodCourt.Service.Mailer;
using FoodCourt.ViewModel;

namespace FoodCourt.Controllers
{
    [AuthorizeRedirectToRegister]
    public class PollController : BaseController
    {
        private string[] _defaultKinds = new string[] { "Pizza", "Pasta", "Chinese", "Homemade", "Sushi" };

        // GET: Poll
        public ActionResult Index()
        {
            ViewBag.IsAdminOfGroup = CurrentGroup.CreatedBy == CurrentUser;
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
                /*
                Why idea of adding new Kinds every time the poll is created instead of GROUP?
                foreach (var singleDefaultKind in _defaultKinds)
                {
                    await UnitOfWork.KindRepository.Insert(new Kind()
                    {
                        CreatedBy = (ApplicationUser)CurrentUser,
                        Group = CurrentGroup,
                        CreatedAt = DateTime.Now,
                        Name = singleDefaultKind
                    });
                }*/
            }

            return Json(currentPollViewModel, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Finish()
        {
            Poll poll = await UnitOfWork.PollRepository.GetCurrentForGroup(CurrentGroup, "CreatedBy").SingleAsync();

            if (poll.CreatedBy.Id != CurrentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "Unauthorized");
            }

            List<Order> orders = UnitOfWork.OrderRepository.GetForPoll(poll).ToList();
            List<OrderBasket> matches = new List<OrderBasket>();

            if (orders.Count > 0)
            {
                OrderMatchHandler matchHandler = new OrderMatchHandler(orders);
                matchHandler.ProcessOrders();

                // add not matched orders
                matches = matchHandler.AddNotMatchedOrders();
            }

            if (poll.IsFinished) // is it "second round"?
            {
                if (poll.IsResolved)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Poll already closed");
                }

                // to prevent "holding up" the poll, second attempt is always resolving it
                poll.IsResolved = true;
                await ProcessResolvedPoll(orders, matches);
            }
            else
            {
                poll.IsFinished = true;

                // mark poll as resolved only when there are no non-matched orders
                poll.IsResolved = matches.Any(m => m.IsNotMatched == false && m.MatchedOrders.Count() == 1) == false;

                if (poll.IsResolved)
                {
                    await ProcessResolvedPoll(orders, matches);
                }
                else
                {
                    var singleOrders = matches.Where(o => o.MatchedOrders.Count() == 1 && o.IsNotMatched == false)
                        .SelectMany(o => o.MatchedOrders).ToList();

                    // if poll is not resolved, notify owners of not-matched orders
                    await SendFinalizeWarnings(singleOrders);
                }

            }
            await UnitOfWork.PollRepository.Update(poll);

            // TODO: make PollViewModel constructor that handles rewriting
            return Json(new PollViewModel()
            {
                Id = poll.Id,
                Group = CurrentGroup.Name,
                Orders = null, // i'm too lazy
                ETA = poll.ETA,
                FinishedAt = poll.FinishedAt,
                IsFinished = poll.IsFinished,
                IsResolved = poll.IsResolved,
                Remarks = poll.Remarks
            });
        }

        private async Task ProcessResolvedPoll(List<Order> orders, List<OrderBasket> matches)
        {
            var users = orders.Select(o => o.CreatedBy).Distinct().ToList();
            await SendNotifications(matches, users);
            await UpdateLastOrderDates(matches);
        }

        private async Task UpdateLastOrderDates(List<OrderBasket> matches)
        {
            foreach (OrderBasket orderBasket in matches)
            {
                ApplicationUser captain = orderBasket.Captain;
                if (captain != null)
                {
                    captain.LastOrderDate = DateTime.Now;

                    await UnitOfWork.UserAccountRepository.Update(captain);   
                }
            }
        }

        private async Task SendFinalizeWarnings(List<Order> singleOrders)
        {
            UrlHelper urlHelper = new UrlHelper(ControllerContext.RequestContext);
            List<ApplicationUser> recipients = new List<ApplicationUser>();
            List<EmailDTO> emailDtos = new List<EmailDTO>();

            foreach (Order order in singleOrders)
            {
                ApplicationUser orderOwner = order.CreatedBy;
                recipients.Add(orderOwner);

                emailDtos.Add(new EmailDTO()
                {
                    PollUrl = urlHelper.Action("Index", "Poll", new {}, ControllerContext.RequestContext.HttpContext.Request.Url.Scheme)
                });
            }

            await Postman.Send("OrderWarning", recipients.Select(u => u.Email).ToList(), emailDtos);
        }

        private async Task SendNotifications(List<OrderBasket> baskets, List<ApplicationUser> users)
        {
            UrlHelper urlHelper = new UrlHelper(ControllerContext.RequestContext);
            List<ApplicationUser> recipients = users;
            List<EmailDTO> emailDtos = new List<EmailDTO>();

            for (var i = 0; i < recipients.Count; i++)
            {
                emailDtos.Add(new EmailDTO()
                {
                    RecipientName = recipients[i].Email,
                    PollUrl = urlHelper.Action("Index", "Poll", new { }, ControllerContext.RequestContext.HttpContext.Request.Url.Scheme),
                    Baskets = baskets
                });
            }

            await Postman.Send("OrderNotification", recipients.Select(u => u.Email).ToList(), emailDtos);
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
