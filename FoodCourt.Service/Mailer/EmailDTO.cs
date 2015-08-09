using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCourt.Service.Mailer
{
    public class EmailDTO
    {
        public string GroupName { get; set; }
        public string GroupOwner { get; set; }

        public string RecipientName { get; set; }
        public string PasswordSetUrl { get; set; }
        public string PollUrl { get; set; }
        public List<OrderBasket> Baskets { get; set; }
    }
}
