using System;
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
            }

            return Json(currentPollViewModel, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> FinishPoll()
        {
            Poll poll = await UnitOfWork.PollRepository.GetCurrentForGroup(CurrentGroup, "Orders, CreatedBy").SingleAsync();

            if (poll.CreatedBy.Id != CurrentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "Unauthorized");
            }

            List<Order> orders = poll.Orders.ToList();
            var matches = OrderMatchHandler.ProcessOrders(orders);

            if (poll.IsFinished)
            {
                if (poll.IsResolved)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Poll already closed");
                }

                poll.IsResolved = true;
                await UnitOfWork.PollRepository.Update(poll);

                // send confirmations
                ProcessFinishedPoll(poll, orders, matches);
            }
            else
            {
                poll.IsFinished = true;
                poll.IsResolved = matches.Any(m => m.MatchedOrders.Count() == 1) == false;

                await UnitOfWork.PollRepository.Update(poll);

                if (poll.IsResolved)
                {
                    ProcessFinishedPoll(poll, orders, matches);
                }
                else
                {
                    var singleOrders = matches.Where(o => o.MatchedOrders.Count() == 1)
                        .SelectMany(o => o.MatchedOrders).ToList();

                    SendFinalizeWarnings(poll, singleOrders);
                }
            }

            return Json("");
        }

        private void ProcessFinishedPoll(Poll poll, List<Order> orders, List<OrderBasket> matches)
        {
            SendNotifications(poll, orders);
            UpdateLastOrderDates(matches);
        }

        private async void UpdateLastOrderDates(List<OrderBasket> matches)
        {
            foreach (OrderBasket orderBasket in matches)
            {
                ApplicationUser captain = orderBasket.Captain;
                captain.LastOrderDate = DateTime.Now;

                await UnitOfWork.UserAccountRepository.Update(captain);
            }
        }

        private void SendFinalizeWarnings(Poll poll, List<Order> singleOrders)
        {
            List<ApplicationUser> recipients = new List<ApplicationUser>();
            List<EmailDTO> emailDtos = new List<EmailDTO>();

            foreach (Order order in singleOrders)
            {
                ApplicationUser orderOwner = order.CreatedBy;
                recipients.Add(orderOwner);

                emailDtos.Add(new EmailDTO()
                {
                    
                });
            }

            Postman.Send("OrderWarning", recipients.Select(u => u.Email).ToList(), emailDtos);
        }

        private void SendNotifications(Poll poll, List<Order> orders)
        {
            List<ApplicationUser> recipients = new List<ApplicationUser>();
            List<EmailDTO> emailDtos = new List<EmailDTO>();

            foreach (Order order in orders)
            {
                ApplicationUser orderOwner = order.CreatedBy;
                recipients.Add(orderOwner);

                emailDtos.Add(new EmailDTO()
                {

                });
            }

            Postman.Send("OrderNotification", recipients.Select(u => u.Email).ToList(), emailDtos);
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
