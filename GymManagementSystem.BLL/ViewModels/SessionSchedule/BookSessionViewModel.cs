using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.BLL.ViewModels.SessionSchedule
{
    public class BookSessionViewModel
    {
        public int SessionId { get; set; }
        public string CategoryName { get; set; } = default!;
        public string TrainerName { get; set; } = default!;
        public string DateDisplay { get; set; } = default!;
        public string TimeRangeDisplay { get; set; } = default!;
        public int AvailableSlots { get; set; }

        [Required(ErrorMessage = "Member is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a member")]
        [Display(Name = "Member")]
        public int MemberId { get; set; }
    }
}
