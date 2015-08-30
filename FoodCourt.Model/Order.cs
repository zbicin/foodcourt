using System;
using FoodCourt.Model.Identity;

namespace FoodCourt.Model
{
    public class Order : BaseEntity
    {
        public Dish Dish { get; set; }
        //public bool IsOptional { get; set; }
        public bool IsHelpNeeded { get; set; }
        public Poll Poll { get; set; }

        public override string ToString()
        {
            return String.Format("{0} by {1}",
                this.Dish.ToString(),
                this.CreatedBy.ToString());
        }
    }
}