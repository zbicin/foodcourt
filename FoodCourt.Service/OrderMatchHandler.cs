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

        // step 1
        // group ALL orders by RestaurantId into baskets

        // step 2
        // try to reduce amount of "baskets" starting from smallest basket

        // step 3
        // try to balance baskets mixing orders from step 1 & 2 starting from biggest basket

        private List<OrderBasket> _orderBaskets;
        private List<OrderBasket> _originalBaskets;
        private List<BasketBalanceRankElement> _balanceResults;
        //private List<OrderBasket> _reducedBaskets;

        public List<OrderBasket> ReducedBaskets
        {
            get { return _orderBaskets; }
        }

        public OrderMatchHandler(List<Order> ordersPool)
        {
            GroupOrdersIntoBaskets(ordersPool);
            //_reducedBaskets = new List<OrderBasket>();
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
                MatchedOrders = g.ToList()
            }).OrderBy(b => b.MatchedOrders.Count()).Select(o => o);
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

    public class OrderBasket
    {
        public Guid RestaurantId;
        public List<Order> MatchedOrders;
        public List<Order> PossibleOrders;

        public bool IsNotMatched = false;

        public OrderBasket()
        {
            MatchedOrders = new List<Order>();
            PossibleOrders = new List<Order>();
        }
    }

    public class BasketBalanceRankElement
    {
        public double Rank;

        public List<OrderBasket> Baskets;

        public BasketBalanceRankElement(List<OrderBasket> baskets)
        {
            Baskets = ObjectCopier.Clone(baskets);

            int basketsCnt = Baskets.Count();
            int spread = Baskets.Max(b => b.MatchedOrders.Count()) - Baskets.Min(b => b.MatchedOrders.Count());

            Rank = spread/(double)basketsCnt;
        }
    }
}
