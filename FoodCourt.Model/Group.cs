using System.Collections.Generic;
using FoodCourt.Model.Identity;

namespace FoodCourt.Model
{
    public class Group : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}