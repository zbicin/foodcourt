using System.Collections.Generic;
using FoodCourt.Model.Identity;
using System.Collections;

namespace FoodCourt.Model
{
    public class Group : BaseEntity
    {
        public Group(string name, ICollection<ApplicationUser> applicationUsers)
        {
            this.Name = name;
            this.ApplicationUsers = applicationUsers;
        }

        public Group() : base() {}

        public string Name { get; set; }
        public ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}