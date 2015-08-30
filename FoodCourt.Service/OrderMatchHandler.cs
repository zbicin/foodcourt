using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodCourt.Model;
using FoodCourt.Model.Identity;

namespace FoodCourt.Service
{
    public class OrderMatchHandler
    {

        private List<Order> _ordersPool;
        private List<OrderBasket> _orderBaskets;
        private List<OrderBasket> _originalBaskets;
        private List<BasketBalanceRankElement> _balanceResults;
        //private List<OrderBasket> _reducedBaskets;

        public OrderMatchHandler(List<Order> ordersPool)
        {
            _ordersPool = ordersPool;
            GroupOrdersIntoBaskets(_ordersPool);
            //_reducedBaskets = new List<OrderBasket>();
        }

        public List<OrderBasket> ProcessOrders()
        {
            // step 1
            // group ALL orders by RestaurantId into baskets

            // step 2
            // try to reduce amount of "baskets" starting from smallest basket

            // step 3
            // try to balance baskets mixing orders from step 1 & 2 starting from biggest basket
            
            ReduceAmountOfBaskets();
            BalanceBaskets();

            PerformElection();

            return _orderBaskets;
        }

        public List<OrderBasket> AddNotMatchedOrders()
        {
            var matchedOrderIds = _orderBaskets.SelectMany(b => b.MatchedOrders).Select(o => o.Id);

            var baskets = new List<OrderBasket>();
            baskets.AddRange(_orderBaskets);

            // add not matched orders
            var notGroupedOrders = _ordersPool.Where(o => matchedOrderIds.Contains(o.Id) == false).ToList();
            baskets.Add(new OrderBasket()
            {
                MatchedOrders = notGroupedOrders,
                IsNotMatched = true
            });

            return baskets;
        }

        private void PerformElection()
        {
            foreach (var orderBasket in _orderBaskets)
            {
                CaptainElectionHelper.PerformReferendum(orderBasket);
            }
        }


        private void GroupOrdersIntoBaskets(List<Order> ordersPool)
        {
            _orderBaskets = ParseInput(ordersPool).ToList();
            _originalBaskets = ParseInput(ordersPool).ToList();
        }

        private IEnumerable<OrderBasket> ParseInput(List<Order> ordersPool)
        {
            return ordersPool.GroupBy(o => o.Dish.Restaurant.Id, o => o).Select(g => new OrderBasket()
            {
                RestaurantId = g.Key,
                MatchedOrders = g.ToList(),
                // sorry bout that combo, see AbilityToGetRidOf description to learn more
                AbilityToGetRidOf = 1.0 * g.Count(r => ordersPool.Any(d => d.CreatedBy.Id == r.CreatedBy.Id && !g.Contains(d))) / g.Count()
            }).OrderByDescending(b => b.AbilityToGetRidOf).ThenBy(b =>b.MatchedOrders.Count).Select(o => o);
        }

        public void ReduceAmountOfBaskets()
        {
            foreach (OrderBasket orderBasket in _orderBaskets)
            {
                ReduceBasketSize(orderBasket);
            }

            CleanupEmptyBaskets();
        }

        private void ReduceBasketSize(OrderBasket orderBasket)
        {
            var reducedBasket = new OrderBasket() {RestaurantId = orderBasket.RestaurantId};

            Order[] orderListClone = new Order[orderBasket.MatchedOrders.Count];
            orderBasket.MatchedOrders.CopyTo(orderListClone);

            foreach (Order order in orderListClone)
            {
                // does owner of this order have other order in another basket?
                var orderOwner = order.CreatedBy;
                if (HasOrderInAnotherBasket(_orderBaskets, orderOwner, orderBasket.RestaurantId))
                {
                    // rewrite order to basket
                    orderBasket.MatchedOrders.Remove(order);
                    orderBasket.PossibleOrders.Add(order);
                }
            }

            // REVIEW: it's always false
            if (reducedBasket.MatchedOrders.Any())
            {
                _orderBaskets.Add(reducedBasket);
            }
        }

        private bool HasOrderInAnotherBasket(IEnumerable<OrderBasket> baskets, ApplicationUser owner, Guid currentRestaurantId)
        {
            return baskets.Any(
                b => b.RestaurantId != currentRestaurantId 
                    && b.MatchedOrders.Any(o => o.CreatedBy.Id == owner.Id));
        }



        public void BalanceBaskets()
        {
            // iterate balancing until baskets remain the same
            _balanceResults = new List<BasketBalanceRankElement>();
            do
            {
                List<OrderBasket> basketsOrderedBySize =
                    _orderBaskets.OrderByDescending(b => b.MatchedOrders.Count()).ToList();

                foreach (OrderBasket orderBasket in basketsOrderedBySize)
                {
                    BalanceBasket(orderBasket);
                }

                _balanceResults.Add(new BasketBalanceRankElement(_orderBaskets));
            } while (!AreBalanced());
        }

        private bool AreBalanced()
        {
            // compare last two results
            int balanceResultsCnt = _balanceResults.Count();
            if (balanceResultsCnt >= 2)
            {
                var last = _balanceResults.ElementAt(balanceResultsCnt - 1);
                var secondToLast = _balanceResults.ElementAt(balanceResultsCnt - 2);

                if (Math.Abs(last.Rank - secondToLast.Rank) < 0.001)
                {
                    return true;
                }
                
                if(last.Rank > secondToLast.Rank)
                {
                    // we achieved worst result than before
                    // restore and finish balancing
                    _orderBaskets = new List<OrderBasket>(secondToLast.Baskets);

                    return true;
                }
            }

            return false;
        }

        private void BalanceBasket(OrderBasket orderBasket)
        {
            Order[] orderListClone = new Order[orderBasket.MatchedOrders.Count];
            orderBasket.MatchedOrders.CopyTo(orderListClone);

            foreach (Order order in orderListClone)
            {
                // did this user have order in other, currently smaller basket?
                var orderOwner = order.CreatedBy;

                IEnumerable<Guid> currentlySmallerBaskets =
                    _orderBaskets.Where(b => (b.MatchedOrders.Count() < orderBasket.MatchedOrders.Count() || (b.MatchedOrders.Count() == orderBasket.MatchedOrders.Count() && orderBasket.RestaurantId != b.RestaurantId)))
                        .OrderByDescending(b => b.MatchedOrders.Count())
                        .Select(b => b.RestaurantId);

                OrderBasket smallerBasket =
                    _orderBaskets.FirstOrDefault(b => currentlySmallerBaskets.Contains(b.RestaurantId) &&
                                                      b.PossibleOrders.Any(o => o.CreatedBy.Id == orderOwner.Id));

                if (smallerBasket != null)
                {
                    // if so, remove order from this basket 
                    orderBasket.MatchedOrders.Remove(order);
                    // while balancing DO NOT save order in PossibleOrders to avoid infinite loops

                    // and rewrite previously removed orders into smaller basket
                    RestoreOrder(smallerBasket, orderOwner);

                    CleanupEmptyBaskets();

                    break; 
                }
            }
        }

        private void RestoreOrder(OrderBasket basket, ApplicationUser owner)
        {
            Order order = basket.PossibleOrders.Single(o => o.CreatedBy.Id == owner.Id);
            basket.PossibleOrders.Remove(order);
            basket.MatchedOrders.Add(order);
        }

        private void CleanupEmptyBaskets()
        {
            _orderBaskets.RemoveAll(b => !b.MatchedOrders.Any());
        }
    }
}
