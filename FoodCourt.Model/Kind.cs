using System;

namespace FoodCourt.Model
{
    public class Kind : BaseEntity
    {
        public Group Group { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}