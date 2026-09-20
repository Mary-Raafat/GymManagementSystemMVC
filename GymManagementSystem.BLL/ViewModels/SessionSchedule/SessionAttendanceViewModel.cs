using System;
using System.Collections.Generic;

namespace GymManagementSystem.BLL.ViewModels.SessionSchedule
{
    public class SessionAttendanceViewModel
    {
        public int SessionId { get; set; }
        public string CategoryName { get; set; } = default!;
        public string TrainerName { get; set; } = default!;
        public string DateDisplay { get; set; } = default!;
        public string TimeRangeDisplay { get; set; } = default!;
        public int Capacity { get; set; }
        public int BookedCount => Bookings.Count;
        public int AvailableSlots => Math.Max(0, Capacity - BookedCount);

        public List<BookingItemViewModel> Bookings { get; set; } = [];
    }

    public class BookingItemViewModel
    {
        public int BookingId { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; } = default!;
        public string MemberPhone { get; set; } = default!;
        public DateTime BookingDate { get; set; }
        public string BookingDateDisplay => BookingDate.ToString("dd MMM yyyy HH:mm");
        public bool IsAttended { get; set; }
    }
}
