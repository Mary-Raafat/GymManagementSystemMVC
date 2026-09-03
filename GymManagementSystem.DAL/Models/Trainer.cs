using GymManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Models
{
    public  class Trainer:User
    {
        public DateTime HireDate { get; set; }
        public Speciality Speciality { get; set; }
        public ICollection<Session> Sessions { get; set; } =[];
    }
}
