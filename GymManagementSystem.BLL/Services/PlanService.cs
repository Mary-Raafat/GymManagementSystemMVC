using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Mapping;
using GymManagementSystem.BLL.ViewModels.Plan;
using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Repositories;
using GymManagementSystem.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public class PlanService(IUnitOfWork unitOfWork, ILogger<PlanService> logger) : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<PlanService> _logger = logger;

        public async Task<IEnumerable<PlanViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.Plans.GetAllAsync(cancellationToken: ct);
            return plans.Select(p => p.ToViewModel());
        }

        public async Task<PlanDetailsViewModel?> GetDetailsAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.Plans.GetByIdAsync(id, cancellationToken: ct);
            if (plan == null)
            {
                _logger.LogWarning("Plan with ID {PlanId} not found.", id);
                return null;
            }

            return plan.ToDetailsViewModel();
        }

        public async Task<EditPlanViewModel?> GetForEditAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.Plans.GetByIdAsync(id, cancellationToken: ct);
            if (plan == null)
            {
                _logger.LogWarning("Plan with ID {PlanId} not found for editing.", id);
                return null;
            }

            return plan.ToEditViewModel();
        }

        public async Task<Result> UpdateAsync(EditPlanViewModel viewModel, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.Plans.GetByIdAsync(viewModel.Id, cancellationToken: ct);
            if (plan == null)
            {
                _logger.LogWarning("Plan update failed: Plan ID {PlanId} not found.", viewModel.Id);
                return Result.Failure("Plan not found.", nameof(viewModel.Id));
            }

            var normalizedDescription = viewModel.Description.Trim();
            bool isDescChanged = plan.Description != normalizedDescription;
            bool isDurationChanged = plan.DurationDays != viewModel.DurationDays;
            bool isPriceChanged = plan.Price != viewModel.Price;

            if (!isDescChanged && !isDurationChanged && !isPriceChanged)
            {
                _logger.LogInformation("No changes detected for plan ID: {PlanId}", viewModel.Id);
                return Result.Failure("No changes were made.");
            }

            plan.Description = normalizedDescription;
            plan.DurationDays = viewModel.DurationDays;
            plan.Price = viewModel.Price;

            _unitOfWork.Plans.Update(plan);
            var rowsAffected = await _unitOfWork.CompleteAsync(ct);
            if (rowsAffected == 0)
            {
                _logger.LogError("Failed to persist updates for plan ID: {PlanId}", viewModel.Id);
                return Result.Failure("Failed to update plan.");
            }

            _logger.LogInformation("Plan ID: {PlanId} updated successfully.", viewModel.Id);
            return Result.Success();
        }

        public async Task<Result> ToggleStatusAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.Plans.GetByIdAsync(id, cancellationToken: ct);
            if (plan == null)
            {
                _logger.LogWarning("Plan status toggle failed: Plan ID {PlanId} not found.", id);
                return Result.Failure("Plan not found.", nameof(id));
            }

            plan.IsActive = !plan.IsActive;
            _unitOfWork.Plans.Update(plan);
            var rowsAffected = await _unitOfWork.CompleteAsync(ct);
            if (rowsAffected == 0)
            {
                _logger.LogError("Failed to toggle status for plan ID: {PlanId}", id);
                return Result.Failure("Failed to update plan status.");
            }

            _logger.LogInformation("Plan ID: {PlanId} status successfully toggled to {IsActive}.", id, plan.IsActive);
            return Result.Success();
        }
    }
}
