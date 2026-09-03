using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Repositories;
using GymManagementSystem.Dbcontexts;
using GymManagementSystem.Models;

namespace GymManagementSystem.DAL.Implementation
{
    public class PlanRepository : GenericRepo<Plan>, IPlanRepository
    {
        public PlanRepository(GymContext dbcontext) : base(dbcontext)
        {

        }
    }
}
