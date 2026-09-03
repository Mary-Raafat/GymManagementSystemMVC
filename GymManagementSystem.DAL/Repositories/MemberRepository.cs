using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.Dbcontexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Implementation
{
    public class MemberRepository(GymContext context) : GenericRepo<Member>(context),IMemberRepository
    {

       
    }
}
