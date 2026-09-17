using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Members;
using GymManagementSystem.BLL.ViewModels.Trainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllAsync(CancellationToken ct = default);
        Task<Result> CreateAsync(CreateTrainerViewModel viewModel, CancellationToken ct = default);
        Task<TrainerDetailsViewModel?> GetDetailsAsync(int id,  CancellationToken ct = default);

        Task<EditTrainerViewModel?> GetForEditAsync(int id, CancellationToken ct = default);

        Task<Result> UpdateAsync(EditTrainerViewModel viewModel, CancellationToken cancellationToken);
        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);


    }
}
