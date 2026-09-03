using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Models
{
    public class Member:User // User inherits from baseEntity
    {
        public string? Photo { get; set; }
        public DateTime JoinDate { get; set; }

        public HealthRecord HealthRecord { get; set; }=null!;


        public ICollection<Booking> Bookings { get; set; } = [];
        public ICollection<Membership> Memberships { get; set; } = [];
    }
}
