using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Membership;
using GymManagementSystem.BLL.ViewModels.SessionSchedule;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public interface ISessionScheduleService
    {
        Task<IEnumerable<SessionScheduleViewModel>> GetUpcomingScheduleAsync(CancellationToken ct = default);
        Task<BookSessionViewModel?> GetForBookingAsync(int sessionId, CancellationToken ct = default);
        Task<Result> BookMemberAsync(BookSessionViewModel viewModel, CancellationToken ct = default);
        Task<SessionAttendanceViewModel?> GetSessionBookingsAsync(int sessionId, CancellationToken ct = default);
        Task<Result> ToggleAttendanceAsync(int bookingId, CancellationToken ct = default);
        Task<Result> CancelBookingAsync(int bookingId, CancellationToken ct = default);
        Task<IEnumerable<MembershipLookupViewModel>> GetEligibleMembersLookupAsync(int sessionId, CancellationToken ct = default);
    }
}
