using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Mapping;
using GymManagementSystem.BLL.ViewModels.Membership;
using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Repositories;
using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public class MembershipService(IUnitOfWork unitOfWork, ILogger<MembershipService> logger) : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<MembershipService> _logger = logger;

        public async Task<IEnumerable<MembershipViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var memberships = await _unitOfWork.Memberships.GetAllAsync(
                include: query => query.Include(m => m.Member)
                                       .Include(m => m.Plan),
                cancellationToken: ct);

            return memberships.Select(m => m.ToViewModel());
        }

        public async Task<Result> CreateAsync(CreateMembershipViewModel viewModel, CancellationToken ct = default)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(viewModel.MemberId, cancellationToken: ct);
            if (member == null)
            {
                _logger.LogWarning("Membership creation failed: Member ID {MemberId} not found.", viewModel.MemberId);
                return Result.Failure("Selected member does not exist.", nameof(CreateMembershipViewModel.MemberId));
            }

            var plan = await _unitOfWork.Plans.GetByIdAsync(viewModel.PlanId, cancellationToken: ct);
            if (plan == null)
            {
                _logger.LogWarning("Membership creation failed: Plan ID {PlanId} not found.", viewModel.PlanId);
                return Result.Failure("Selected plan does not exist.", nameof(CreateMembershipViewModel.PlanId));
            }

            if (!plan.IsActive)
            {
                _logger.LogWarning("Membership creation failed: Plan ID {PlanId} is currently inactive.", viewModel.PlanId);
                return Result.Failure("Selected plan is currently inactive.", nameof(CreateMembershipViewModel.PlanId));
            }

            if (await _unitOfWork.Memberships.HasActiveMembershipAsync(viewModel.MemberId, ct))
            {
                _logger.LogWarning("Membership creation failed: Member ID {MemberId} already has an active membership.", viewModel.MemberId);
                return Result.Failure("This member already has an active membership.", nameof(CreateMembershipViewModel.MemberId));
            }

            var endDate = viewModel.StartDate.AddDays(plan.DurationDays);
            var membership = viewModel.ToEntity(endDate);

            await _unitOfWork.Memberships.AddAsync(membership, ct);
            var rowsAffected = await _unitOfWork.CompleteAsync(ct);
            if (rowsAffected == 0)
            {
                _logger.LogError("Failed to persist membership for member ID: {MemberId} in the database.", viewModel.MemberId);
                return Result.Failure("Failed to create membership.");
            }

            _logger.LogInformation("Membership created successfully for member ID: {MemberId} with plan ID: {PlanId}.", viewModel.MemberId, viewModel.PlanId);
            return Result.Success();
        }

        public async Task<Result> CancelAsync(int id, CancellationToken ct = default)
        {
            var membership = await _unitOfWork.Memberships.GetByIdAsync(id, cancellationToken: ct);
            if (membership == null)
            {
                _logger.LogWarning("Membership cancellation failed: Membership ID {MembershipId} not found.", id);
                return Result.Failure("Membership not found.", nameof(id));
            }

            await _unitOfWork.Memberships.SoftDeleteAsync(membership, ct);
            var rowsAffected = await _unitOfWork.CompleteAsync(ct);
            if (rowsAffected == 0)
            {
                _logger.LogError("Failed to cancel membership ID: {MembershipId} in the database.", id);
                return Result.Failure("Failed to cancel membership.");
            }

            _logger.LogInformation("Membership ID: {MembershipId} cancelled successfully.", id);
            return Result.Success();
        }

        public async Task<IEnumerable<MembershipLookupViewModel>> GetMembersLookupAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.Members.GetAllAsync(cancellationToken: ct);
            return members.Select(m => m.ToMemberLookupViewModel());
        }

        public async Task<IEnumerable<MembershipLookupViewModel>> GetPlansLookupAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.Plans.GetAllAsync(cancellationToken: ct);
            return plans.Where(p => p.IsActive).Select(p => p.ToPlanLookupViewModel());
        }


     
    }
}
