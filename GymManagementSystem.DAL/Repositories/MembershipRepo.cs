using GymManagementSystem.DAL.Implementation;
using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.Dbcontexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Repositories
{
    public class MembershipRepo(GymContext context) : GenericRepo<Membership>(context), IMembershipRepo
    {
        private readonly GymContext _context = context;

        public Task<bool> HasActiveMembershipAsync(int memberId, CancellationToken ct = default)
        {
            return _context.Memberships
                .AnyAsync(m => m.MemberId == memberId && m.EndDate >= DateTime.UtcNow, ct);
        }
    }
}
