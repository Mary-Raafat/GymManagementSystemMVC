using GymManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Models
{
    public class Membership:BaseEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;


        public int PlanId { get; set; }
        public Plan Plan { get; set; } = null!;

    }
}
