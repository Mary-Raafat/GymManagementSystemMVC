using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.BLL.ViewModels.Plan
{
    public class EditPlanViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 200 characters")]
        public string Description { get; set; } = default!;

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
        [Display(Name = "Duration (Days)")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 100000, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
    }
}
