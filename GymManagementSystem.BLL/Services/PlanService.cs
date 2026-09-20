using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Plan;
using GymManagementSystem.DAL.Repositories;
using GymManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public class PlanService(IPlanRepository planRepository) : IPlanService
    {
        private readonly IPlanRepository _planRepository = planRepository;

        public async Task<IEnumerable<PlanViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var plans = await _planRepository.GetAllAsync(cancellationToken: ct);
            return plans.Select(p => new PlanViewModel
            {
                Id = p.ID,
                Name = p.Name,
                Description = p.Description,
                DurationDays = p.DurationDays,
                Price = p.Price,
                IsActive = p.IsActive
            });
        }

        public async Task<PlanDetailsViewModel?> GetDetailsAsync(int id, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetByIdAsync(id, cancellationToken: ct);
            if (plan == null)
            {
                return null;
            }

            return new PlanDetailsViewModel
            {
                Id = plan.ID,
                Name = plan.Name,
                Description = plan.Description,
                DurationDays = plan.DurationDays,
                Price = plan.Price,
                IsActive = plan.IsActive
            };
        }

        public async Task<EditPlanViewModel?> GetForEditAsync(int id, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetByIdAsync(id, cancellationToken: ct);
            if (plan == null)
            {
                return null;
            }

            return new EditPlanViewModel
            {
                Id = plan.ID,
                Name = plan.Name,
                Description = plan.Description,
                DurationDays = plan.DurationDays,
                Price = plan.Price
            };
        }

        public async Task<Result> UpdateAsync(EditPlanViewModel viewModel, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetByIdAsync(viewModel.Id, cancellationToken: ct);
            if (plan == null)
            {
                return Result.Failure("Plan not found.", nameof(viewModel.Id));
            }

            var normalizedDescription = viewModel.Description.Trim();
            bool isDescChanged = plan.Description != normalizedDescription;
            bool isDurationChanged = plan.DurationDays != viewModel.DurationDays;
            bool isPriceChanged = plan.Price != viewModel.Price;

            if (!isDescChanged && !isDurationChanged && !isPriceChanged)
            {
                return Result.Failure("No changes were made.");
            }

            plan.Description = normalizedDescription;
            plan.DurationDays = viewModel.DurationDays;
            plan.Price = viewModel.Price;

            _planRepository.Update(plan);
            var rowsAffected = await _planRepository.SaveChangesAsync(ct);
            if (rowsAffected == 0)
            {
                return Result.Failure("Failed to update plan.");
            }

            return Result.Success();
        }

        public async Task<Result> ToggleStatusAsync(int id, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetByIdAsync(id, cancellationToken: ct);
            if (plan == null)
            {
                return Result.Failure("Plan not found.", nameof(id));
            }

            plan.IsActive = !plan.IsActive;
            _planRepository.Update(plan);
            var rowsAffected = await _planRepository.SaveChangesAsync(ct);
            if (rowsAffected == 0)
            {
                return Result.Failure("Failed to update plan status.");
            }

            return Result.Success();
        }
    }
}
