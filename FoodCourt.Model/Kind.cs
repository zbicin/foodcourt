namespace FoodCourt.Model
{
    public class Kind : BaseEntity
    {
        public Group Group { get; set; }
        public string Name { get; set; }
    }
}