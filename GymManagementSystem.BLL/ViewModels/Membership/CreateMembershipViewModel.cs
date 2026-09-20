using System;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.BLL.ViewModels.Membership
{
    public class CreateMembershipViewModel
    {
        [Required(ErrorMessage = "Member is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid member")]
        [Display(Name = "Member")]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Plan is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid plan")]
        [Display(Name = "Plan")]
        public int PlanId { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;
    }
}
