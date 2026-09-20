using System;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.BLL.ViewModels.Session
{
    public class CreateSessionViewModel
    {
        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 500 characters")]
        public string Description { get; set; } = default!;

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 25, ErrorMessage = "Capacity must be between 1 and 25")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "End Date is required")]
        [DataType(DataType.DateTime)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddHours(1);

        [Required(ErrorMessage = "Trainer is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid trainer")]
        [Display(Name = "Trainer")]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
    }
}
