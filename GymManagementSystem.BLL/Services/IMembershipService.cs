using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Membership;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public interface IMembershipService
    {
        Task<IEnumerable<MembershipViewModel>> GetAllAsync(CancellationToken ct = default);
        Task<Result> CreateAsync(CreateMembershipViewModel viewModel, CancellationToken ct = default);
        Task<Result> CancelAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<MembershipLookupViewModel>> GetMembersLookupAsync(CancellationToken ct = default);
        Task<IEnumerable<MembershipLookupViewModel>> GetPlansLookupAsync(CancellationToken ct = default);
    }
}
