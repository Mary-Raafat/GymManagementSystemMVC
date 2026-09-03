using GymManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Models
{
    public  class HealthRecord:BaseEntity
    {
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public BloodType BloodType { get; set; }
        public string? Notes { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;

    }
}
