using GymManagementSystem.BLL.ViewModels.Session;
using GymManagementSystem.DAL.Models;
using System;

namespace GymManagementSystem.BLL.Mapping
{
    public static class SessionMappingExtensions
    {
        // Session → SessionViewModel (قائمة الجلسات)
        public static SessionViewModel ToViewModel(this Session session)
        {
            return new SessionViewModel
            {
                Id = session.ID,
                Description = session.Description,
                Capacity = session.Capacity,
                StartDate = session.StartDate,
                EndDate = session.EndDate,
                TrainerId = session.TrainerId,
                TrainerName = session.Trainer != null ? session.Trainer.Name : "N/A",
                CategoryId = session.CategoryId,
                CategoryName = session.Category != null ? session.Category.Name : "N/A",
                BookedSlots = session.Bookings?.Count ?? 0
            };
        }

        // Session → SessionDetailsViewModel (تفاصيل الجلسة)
        public static SessionDetailsViewModel ToDetailsViewModel(this Session session)
        {
            var bookedCount = session.Bookings?.Count ?? 0;
            var status = DateTime.UtcNow < session.StartDate
                ? "Upcoming"
                : (DateTime.UtcNow <= session.EndDate ? "Ongoing" : "Completed");

            return new SessionDetailsViewModel
            {
                Id = session.ID,
                Description = session.Description,
                Capacity = session.Capacity,
                StartDate = session.StartDate.ToString("yyyy-MM-dd HH:mm"),
                EndDate = session.EndDate.ToString("yyyy-MM-dd HH:mm"),
                TrainerId = session.TrainerId,
                TrainerName = session.Trainer != null ? session.Trainer.Name : "N/A",
                CategoryId = session.CategoryId,
                CategoryName = session.Category != null ? session.Category.Name : "N/A",
                BookedSlots = bookedCount,
                AvailableSlots = Math.Max(0, session.Capacity - bookedCount),
                Status = status,
                CreatedAt = session.CreatedAt.ToString("yyyy-MM-dd HH:mm")
            };
        }

        // Session → EditSessionViewModel (للتعديل)
        public static EditSessionViewModel ToEditViewModel(this Session session)
        {
            return new EditSessionViewModel
            {
                Id = session.ID,
                Description = session.Description,
                Capacity = session.Capacity,
                StartDate = session.StartDate,
                EndDate = session.EndDate,
                TrainerId = session.TrainerId,
                CategoryId = session.CategoryId
            };
        }

        // CreateSessionViewModel → Session (إنشاء جلسة جديدة)
        public static Session ToEntity(this CreateSessionViewModel viewModel)
        {
            return new Session
            {
                Description = viewModel.Description.Trim(),
                Capacity = viewModel.Capacity,
                StartDate = viewModel.StartDate,
                EndDate = viewModel.EndDate,
                TrainerId = viewModel.TrainerId,
                CategoryId = viewModel.CategoryId
            };
        }
    }
}
