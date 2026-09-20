using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel>> GetAllAsync(CancellationToken ct = default);
        Task<Result> CreateAsync(CreateSessionViewModel viewModel, CancellationToken ct = default);
        Task<SessionDetailsViewModel?> GetDetailsAsync(int id, CancellationToken ct = default);
        Task<EditSessionViewModel?> GetForEditAsync(int id, CancellationToken ct = default);
        Task<Result> UpdateAsync(EditSessionViewModel viewModel, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<SessionLookupViewModel>> GetTrainersLookupAsync(CancellationToken ct = default);
        Task<IEnumerable<SessionLookupViewModel>> GetCategoriesLookupAsync(CancellationToken ct = default);
    }
}
