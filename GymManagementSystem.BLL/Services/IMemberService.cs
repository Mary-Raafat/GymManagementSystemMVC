using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public interface IMemberService
    {
         Task<IEnumerable<MemberViewModel>> GetAllAsync(CancellationToken ct = default);
         Task<Result> CreateAsync(CreateMemberViewModel viewModel, CancellationToken ct = default);

         Task<MemberDetailsViewModel?> GetDetailsAsync(int id, CancellationToken ct = default);
         Task<HealthRecordDetailsViewModel?> GetHealthRecordDetailsAsync(int memberId, CancellationToken ct = default);

        Task<EditMemberViewModel?> GetForEditAsync(int id, CancellationToken ct = default);
        Task<Result> UpdateAsync(EditMemberViewModel viewModel, CancellationToken cancellationToken);

    }
}
