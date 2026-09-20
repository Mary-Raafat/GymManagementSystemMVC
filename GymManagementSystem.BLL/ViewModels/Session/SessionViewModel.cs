using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.ViewModels.Session
{
    public class SessionViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = default!;
        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TrainerId { get; set; }
        public string TrainerName { get; set; } = default!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = default!;
        public int BookedSlots { get; set; }
        public int AvailableSlots => Math.Max(0, Capacity - BookedSlots);
        public string Status => DateTime.UtcNow < StartDate ? "Upcoming" : (DateTime.UtcNow <= EndDate ? "Ongoing" : "Completed");

        public string DateDisplay => StartDate.ToString("dd MMM yyyy");
        public string TimeRangeDisplay => $"{StartDate:hh:mm tt} - {EndDate:hh:mm tt}";
        public string Duration
        {
            get
            {
                var duration = EndDate - StartDate;
                if (duration.TotalMinutes <= 0) return "0 min";
                if (duration.TotalHours < 1) return $"{duration.Minutes} mins";
                if (duration.Minutes == 0) return $"{(int)duration.TotalHours} hr";
                return $"{(int)duration.TotalHours} hr {duration.Minutes} mins";
            }
        }
    }
}
