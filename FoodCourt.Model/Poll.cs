using System;
using System.Collections.Generic;

namespace FoodCourt.Model
{
    public class Poll : BaseEntity
    {
        public ICollection<Order> Orders { get; set; }

        public DateTime ETA { get; set; }
        public string Remarks { get; set; }
        public bool IsFinished { get; set; }
        public DateTime FinishedAt { get; set; }
        public bool IsResolved { get; set; }
    }
}