using System.Collections.Generic;
using System.Linq;

namespace FoodCourt.Service
{
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