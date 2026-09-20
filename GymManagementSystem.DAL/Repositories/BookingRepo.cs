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
    public class BookingRepo(GymContext context) : GenericRepo<Booking>(context), IBookingRepo
    {
        private readonly GymContext _context = context;

        public Task<bool> IsMemberBookedInSessionAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            return _context.Bookings
                .AnyAsync(b => b.MemberId == memberId && b.SessionId == sessionId, ct);
        }

        public Task<bool> HasMemberConflictBookingAsync(int memberId, DateTime startDate, DateTime endDate, CancellationToken ct = default)
        {
            return _context.Bookings
                .Include(b => b.Session)
                .AnyAsync(b => b.MemberId == memberId
                            && b.Session.StartDate < endDate
                            && b.Session.EndDate > startDate, ct);
        }
    }
}
