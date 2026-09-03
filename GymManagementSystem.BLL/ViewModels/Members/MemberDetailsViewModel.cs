using GymManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.ViewModels.Members
{
    public class MemberDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? PhotoUrl { get; set; }
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public Gender Gender { get; set; }
        public string DateOfBirth { get; set; } = null!;
        public string Address { get; set; } = default!;
        public string PlanName { get; set; } = default!;
        public string MemberShipStartDate { get; set; } = null!;
        public string MemberShipEndDate { get; set; } = null!;



    }
}
