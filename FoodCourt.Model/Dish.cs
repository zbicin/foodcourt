using System;

namespace FoodCourt.Model
{
    public class Dish : BaseEntity
    {
        public Group Group { get; set; }
        public string Name { get; set; }
        public Kind Kind { get; set; }
        public Restaurant Restaurant { get; set; }

        public override string ToString()
        {
            return String.Format("{0} ({1}) from {2}",
                this.Name, 
                this.Kind.ToString(), 
                this.Restaurant.ToString());
        }
    }
}