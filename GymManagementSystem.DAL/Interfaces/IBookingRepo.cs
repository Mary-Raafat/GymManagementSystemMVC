using GymManagementSystem.DAL.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Interfaces
{
    public interface IBookingRepo : IGenericRepo<Booking>
    {
        Task<bool> IsMemberBookedInSessionAsync(int memberId, int sessionId, CancellationToken ct = default);
        Task<bool> HasMemberConflictBookingAsync(int memberId, DateTime startDate, DateTime endDate, CancellationToken ct = default);
    }
}
