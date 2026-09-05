using GymManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.ViewModels.Members
{
    public  class HealthRecordDetailsViewModel
    {
        public decimal Height { get; set; }

        public decimal Weight { get; set; }
        public string BloodType { get; set; } = default!;
        public string? Notes { get; set; } = default!;





    }
}
