using System;

namespace GymManagementSystem.BLL.ViewModels.Session
{
    public class SessionDetailsViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = default!;
        public int Capacity { get; set; }
        public string StartDate { get; set; } = default!;
        public string EndDate { get; set; } = default!;
        public int TrainerId { get; set; }
        public string TrainerName { get; set; } = default!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = default!;
        public int BookedSlots { get; set; }
        public int AvailableSlots { get; set; }
        public string Status { get; set; } = default!;
        public string CreatedAt { get; set; } = default!;
    }
}
