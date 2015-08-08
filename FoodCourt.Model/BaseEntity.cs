using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FoodCourt.Model.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FoodCourt.Model
{
    public class BaseEntity : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid Id { get; set; }

        [Required]
        [Index]
        public DateTime CreatedAt { get; set; }
        [Required]
        public ApplicationUser CreatedBy { get; set; }

        [Index]
        public DateTime? UpdatedAt { get; set; }
        public ApplicationUser UpdatedBy { get; set; }

        [Required]
        [Index]
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public ApplicationUser DeletedBy { get; set; }


        public BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }
    }
}
