using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models
{
    public class TimeSlot
    {
        private DateTime StartTime { get; set; }
        private decimal Duration { get; set; }

        public TimeSlot(DateTime start, decimal duration)
        {
            StartTime = start;
            Duration = duration;
        }

        public bool isOverlaping(TimeSlot other)
        {
            double difference = (StartTime - other.StartTime).TotalMinutes;
            return difference <= (double)other.Duration && difference >= -(double)Duration;
        }
    }
}
