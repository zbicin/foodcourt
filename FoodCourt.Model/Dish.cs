namespace FoodCourt.Model
{
    public class Dish : BaseEntity
    {
        public string Name { get; set; }
        public Kind Kind { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}