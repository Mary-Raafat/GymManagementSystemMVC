using System;
using System.Collections.Generic;

namespace GymManagementSystem.BLL.ViewModels.SessionSchedule
{
    public class SessionScheduleViewModel
    {
        public int SessionId { get; set; }
        public string Description { get; set; } = default!;
        public string CategoryName { get; set; } = default!;
        public string TrainerName { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public int BookedCount { get; set; }
        public int AvailableSlots => Math.Max(0, Capacity - BookedCount);

        public string DateDisplay => StartDate.ToString("dd MMM yyyy");
        public string TimeRangeDisplay => $"{StartDate:hh:mm tt} - {EndDate:hh:mm tt}";
        public string Status => DateTime.UtcNow < StartDate ? "Upcoming" : (DateTime.UtcNow <= EndDate ? "Ongoing" : "Completed");
        public bool IsFull => AvailableSlots <= 0;
        public bool CanBook => Status != "Completed" && !IsFull;
    }
}
