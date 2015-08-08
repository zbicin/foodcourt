using System;
using FoodCourt.Model.Identity;

namespace FoodCourt.Model
{
    public interface IBaseEntity
    {
        System.Guid Id { get; set; }
        DateTime CreatedAt { get; set; }
        ApplicationUser CreatedBy { get; set; }
        DateTime? UpdatedAt { get; set; }
        ApplicationUser UpdatedBy { get; set; }
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
        ApplicationUser DeletedBy { get; set; }
    }
}