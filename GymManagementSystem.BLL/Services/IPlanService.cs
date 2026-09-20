using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Plan;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllAsync(CancellationToken ct = default);
        Task<PlanDetailsViewModel?> GetDetailsAsync(int id, CancellationToken ct = default);
        Task<EditPlanViewModel?> GetForEditAsync(int id, CancellationToken ct = default);
        Task<Result> UpdateAsync(EditPlanViewModel viewModel, CancellationToken ct = default);
        Task<Result> ToggleStatusAsync(int id, CancellationToken ct = default);
    }
}
