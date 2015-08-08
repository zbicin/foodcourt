namespace FoodCourt.Model
{
    public class Order : BaseEntity
    {
        public bool IsOptional { get; set; }
        public bool IsHelpNeeded { get; set; }
    }
}