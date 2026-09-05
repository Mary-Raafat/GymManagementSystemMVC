using GymManagementSystem.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.BLL.ViewModels.Members
{
    public class CreateHealthRecordViewModel
    {
        [Range(0.1, 300, ErrorMessage = "Height must be between 0 and 300")]
        public decimal Height { get; set; }

        [Range(0.1, 500, ErrorMessage = "Weight must be between 0 and 500")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Blood Type Is Required")]
        [EnumDataType(typeof(BloodType),ErrorMessage ="Invalid blood type ")] // جديده 
        public BloodType BloodType { get; set; } = default!;
        public string? Note { get; set; } = default!;

    }
}