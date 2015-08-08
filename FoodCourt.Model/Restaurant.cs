namespace FoodCourt.Model
{
    public class Restaurant : BaseEntity
    {
        public Group Group { get; set; }

        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string MenuUrl { get; set; }

        // will it be really useful here?
        //public Kind Kind { get; set; }
    }
}