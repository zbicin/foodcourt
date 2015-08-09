using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCourt.Model.Identity
{
    public class UserChangePasswordToken : BaseEntity
    {
        public ApplicationUser User { get; set; }

        [NotMapped]
        public bool IsValid {
            get
            {
                TimeSpan diff = DateTime.Now.Subtract(CreatedAt);
                return Math.Ceiling(diff.TotalDays) < 7;
            } 
        }
    }
}
