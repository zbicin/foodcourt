using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodCourt.Model;
using FoodCourt.Model.Identity;

namespace FoodCourt.Service
{
    class OrderMatchHandler
    {

        // step 1
        // group ALL orders by RestaurantId into baskets

        // step 2
        // try to reduce amount of "baskets" starting from smallest basket

        // step 3
        // try to balance baskets mixing orders from step 1 & 2 starting from biggest basket

        private List<OrderBasket> _orderBaskets;
        private List<OrderBasket> _reducedBaskets;

        public OrderMatchHandler(List<Order> ordersPool)
        {
            GroupOrdersIntoBaskets(ordersPool);
            _reducedBaskets = new List<OrderBasket>();
        }

        private void GroupOrdersIntoBaskets(List<Order> ordersPool)
        {
            _orderBaskets = ordersPool.GroupBy(o => o.Dish.Restaurant.Id, o => o).Select(g => new OrderBasket()
            {
                RestaurantId = g.Key,
                Orders = g.ToList()
            }).ToList();
        }

        public void ReduceAmountOfBaskets()
        {
            var basketsOrderedBySize = _orderBaskets.OrderBy(b => b.Orders.Count());

            foreach (OrderBasket orderBasket in basketsOrderedBySize)
            {
                ReduceBasketSize(orderBasket);
            }
        }

        private void ReduceBasketSize(OrderBasket orderBasket)
        {
            var reducedBasket = new OrderBasket() {RestaurantId = orderBasket.RestaurantId};

            foreach (Order order in orderBasket.Orders)
            {
                // does owner of this order have other order in another basket?
                var orderOwner = order.CreatedBy;
                if (!HasOrderInAnotherBasket(_reducedBaskets, orderOwner, orderBasket.RestaurantId))
                {
                    // rewrite order to basket
                    reducedBasket.Orders.Add(order);
                }
            }

            if (reducedBasket.Orders.Any())
            {
                _reducedBaskets.Add(orderBasket);
            }
        }

        private bool HasOrderInAnotherBasket(IEnumerable<OrderBasket> baskets, ApplicationUser owner, Guid currentRestaurantId)
        {
            return baskets.Any(
                b => b.RestaurantId != currentRestaurantId 
                    && b.Orders.Any(o => o.CreatedBy.Id == owner.Id));
        }



        public void BalanceBaskets()
        {
            var basketsOrderedBySize = _reducedBaskets.OrderByDescending(b => b.Orders.Count());

            foreach (OrderBasket orderBasket in basketsOrderedBySize)
            {
                BalanceBasket(orderBasket);
            }
        }

        private void BalanceBasket(OrderBasket orderBasket)
        {
            foreach (Order order in orderBasket.Orders)
            {
                // did this user have order in other, currently smaller basket?
                var orderOwner = order.CreatedBy;

                IEnumerable<Guid> currentlySmallerBaskets =
                    _reducedBaskets.Where(b => b.Orders.Count() < orderBasket.Orders.Count())
                        .Select(b => b.RestaurantId);

                OrderBasket smallerBasket =
                    _orderBaskets.FirstOrDefault(b => currentlySmallerBaskets.Contains(b.RestaurantId) &&
                                                      b.Orders.Any(o => o.CreatedBy.Id == orderOwner.Id));

                if (smallerBasket != null)
                {
                    // if so, remove order from this basket 
                    orderBasket.Orders.Remove(order);
                    CleanupEmptyBaskets();

                    // and rewrite previously removed orders into smaller basket
                    smallerBasket.Orders.Add(order);
                }
            }
        }

        private void CleanupEmptyBaskets()
        {
            _reducedBaskets.RemoveAll(b => !b.Orders.Any());
        }
    }
    public class OrderBasket
    {
        public Guid RestaurantId;
        public List<Order> Orders;

        public OrderBasket()
        {
            Orders = new List<Order>();
        }
    }
}
