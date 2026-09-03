using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.ViewModels.Members
{
    public class MemberViewModel
    {

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public string? PhotoUrl { get; set; }
        public DateOnly JoinDate { get; set; }
        public string Gender { get; set; } = null!;

    }
}
