using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCourt.Service
{
    public static class CaptainElectionHelper
    {
        /// <summary>
        /// (☞■ヮ■)☞
        /// </summary>
        /// <param name="basket"></param>
        public static void PerformReferendum(OrderBasket basket)
        {
            var orderOwners = basket.MatchedOrders.Select(o => o.CreatedBy).OrderBy(u => u.LastOrderDate);
            var basketCaptain = orderOwners.First();

            basket.Captain = basketCaptain; // (▀̿Ĺ̯▀̿ ̿)
        }
    }
}
