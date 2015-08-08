using System;
using System.Collections.Generic;

namespace FoodCourt.ViewModel
{
    public class PollViewModel
    {
        public string Group { get; set; }

        public string Remarks { get; set; }
        public IEnumerable<OrderViewModel> Orders { get; set; }

        public DateTime? ETA { get; set; }

        public bool IsFinished { get; set; }
        public DateTime? FinishedAt { get; set; }

        public bool IsResolved { get; set; }
    }
}