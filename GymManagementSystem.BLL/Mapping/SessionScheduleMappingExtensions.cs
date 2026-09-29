using GymManagementSystem.BLL.ViewModels.Membership;
using GymManagementSystem.BLL.ViewModels.SessionSchedule;
using GymManagementSystem.DAL.Models;
using System;
using System.Linq;

namespace GymManagementSystem.BLL.Mapping
{
    public static class SessionScheduleMappingExtensions
    {
        // Session → SessionScheduleViewModel (جدول الجلسات القادمة)
        public static SessionScheduleViewModel ToScheduleViewModel(this Session session)
        {
            return new SessionScheduleViewModel
            {
                SessionId = session.ID,
                Description = session.Description,
                CategoryName = session.Category?.Name ?? "N/A",
                TrainerName = session.Trainer?.Name ?? "N/A",
                StartDate = session.StartDate,
                EndDate = session.EndDate,
                Capacity = session.Capacity,
                BookedCount = session.Bookings?.Count ?? 0
            };
        }

        // Session → BookSessionViewModel (حجز جلسة)
        public static BookSessionViewModel ToBookViewModel(this Session session)
        {
            var bookedCount = session.Bookings?.Count ?? 0;

            return new BookSessionViewModel
            {
                SessionId = session.ID,
                CategoryName = session.Category?.Name ?? "N/A",
                TrainerName = session.Trainer?.Name ?? "N/A",
                DateDisplay = session.StartDate.ToString("dd MMM yyyy"),
                TimeRangeDisplay = $"{session.StartDate:hh:mm tt} - {session.EndDate:hh:mm tt}",
                AvailableSlots = Math.Max(0, session.Capacity - bookedCount)
            };
        }

        // Session → SessionAttendanceViewModel (الحضور)
        public static SessionAttendanceViewModel ToAttendanceViewModel(this Session session)
        {
            return new SessionAttendanceViewModel
            {
                SessionId = session.ID,
                CategoryName = session.Category?.Name ?? "N/A",
                TrainerName = session.Trainer?.Name ?? "N/A",
                DateDisplay = session.StartDate.ToString("dd MMM yyyy"),
                TimeRangeDisplay = $"{session.StartDate:hh:mm tt} - {session.EndDate:hh:mm tt}",
                Capacity = session.Capacity,
                Bookings = session.Bookings.Select(b => b.ToBookingItemViewModel()).ToList()
            };
        }

        // Booking → BookingItemViewModel
        public static BookingItemViewModel ToBookingItemViewModel(this Booking booking)
        {
            return new BookingItemViewModel
            {
                BookingId = booking.ID,
                MemberId = booking.MemberId,
                MemberName = booking.Member?.Name ?? "N/A",
                MemberPhone = booking.Member?.Phone ?? "N/A",
                BookingDate = booking.Date,
                IsAttended = booking.IsAttended
            };
        }

        // BookSessionViewModel → Booking (حجز عضو في جلسة)
        public static Booking ToBookingEntity(this BookSessionViewModel viewModel)
        {
            return new Booking
            {
                SessionId = viewModel.SessionId,
                MemberId = viewModel.MemberId,
                Date = DateTime.UtcNow,
                IsAttended = false
            };
        }

        // Member → MembershipLookupViewModel (قائمة الأعضاء المؤهلين)
        public static MembershipLookupViewModel ToEligibleMemberLookup(this Member member)
        {
            return new MembershipLookupViewModel
            {
                Id = member.ID,
                Name = $"{member.Name} ({member.Phone})"
            };
        }
    }
}
