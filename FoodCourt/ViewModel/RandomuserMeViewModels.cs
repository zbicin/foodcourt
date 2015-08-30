using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FoodCourt.Controllers;

namespace FoodCourt.ViewModel
{
    public class RandomuserMeViewModels
    {
        public class RandomuserMeResponse
        {
            public List<RandomuserMeResult> Results { get; set; }
        }

        public class RandomuserMeResult
        {
            public RandomuserMeUser User { get; set; }


        }
        public class RandomuserMeUser
        {
            public string Email { get; set; }
        }
    }
}