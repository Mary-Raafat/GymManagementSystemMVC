using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Models.ValueOfObjects
{
    public  class Address
    {
        public string City { get; set; } = null!;
        public string Street { get; set; }=null!;
        public int  BuildingNumber { get; set; }
    }
}
