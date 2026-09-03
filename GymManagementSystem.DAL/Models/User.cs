using GymManagementSystem.DAL.Enums;
using GymManagementSystem.DAL.Models.ValueOfObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Models
{
    public  class User:BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; } //بيتخزن في الداتا بيز ك ارقام 
        //you can convert it into string by hasConversion 

        public Address Address { get; set; }=null!;
    }
}
