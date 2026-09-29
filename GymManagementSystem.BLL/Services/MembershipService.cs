using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Membership;
using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Repositories;
using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace GymManagementSystem.BLL.Services
{
    public class MembershipService(
      IUnitOfWork unitOfWork) : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IEnumerable<MembershipViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var memberships = await _unitOfWork.Memberships.GetAllAsync(
                include: query => query.Include(m => m.Member)
                                       .Include(m => m.Plan),
                cancellationToken: ct);

            return memberships.Select(m => new MembershipViewModel
            {
                Id = m.ID,
                MemberId = m.MemberId,
                MemberName = m.Member?.Name ?? "N/A",
                MemberPhone = m.Member?.Phone ?? "N/A",
                PlanId = m.PlanId,
                PlanName = m.Plan?.Name ?? "N/A",
                PlanPrice = m.Plan?.Price ?? 0,
                StartDate = m.StartDate,
                EndDate = m.EndDate
            });
        }

        public async Task<Result> CreateAsync(CreateMembershipViewModel viewModel, CancellationToken ct = default)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(viewModel.MemberId, cancellationToken: ct);
            if (member == null)
            {
                return Result.Failure("Selected member does not exist.", nameof(CreateMembershipViewModel.MemberId));
            }

            var plan = await _unitOfWork.Plans.GetByIdAsync(viewModel.PlanId, cancellationToken: ct);
            if (plan == null)
            {
                return Result.Failure("Selected plan does not exist.", nameof(CreateMembershipViewModel.PlanId));
            }

            if (!plan.IsActive)
            {
                return Result.Failure("Selected plan is currently inactive.", nameof(CreateMembershipViewModel.PlanId));
            }

            if (await _unitOfWork.Memberships.HasActiveMembershipAsync(viewModel.MemberId, ct))
            {
                return Result.Failure("This member already has an active membership.", nameof(CreateMembershipViewModel.MemberId));
            }

            var membership = new Membership
            {
                MemberId = viewModel.MemberId,
                PlanId = viewModel.PlanId,
                StartDate = viewModel.StartDate,
                EndDate = viewModel.StartDate.AddDays(plan.DurationDays)
            };

            await _unitOfWork.Memberships.AddAsync(membership, ct);
            var rowsAffected = await _unitOfWork.CompleteAsync(ct);
            if (rowsAffected == 0)
            {
                return Result.Failure("Failed to create membership.");
            }

            return Result.Success();
        }

        public async Task<Result> CancelAsync(int id, CancellationToken ct = default)
        {
            var membership = await _unitOfWork.Memberships.GetByIdAsync(id, cancellationToken: ct);
            if (membership == null)
            {
                return Result.Failure("Membership not found.", nameof(id));
            }

            await _unitOfWork.Memberships.SoftDeleteAsync(membership, ct);
            var rowsAffected = await _unitOfWork.CompleteAsync(ct);
            if (rowsAffected == 0)
            {
                return Result.Failure("Failed to cancel membership.");
            }

            return Result.Success();
        }

        public async Task<IEnumerable<MembershipLookupViewModel>> GetMembersLookupAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.Members.GetAllAsync(cancellationToken: ct);
            return members.Select(m => new MembershipLookupViewModel
            {
                Id = m.ID,
                Name = $"{m.Name} ({m.Phone})"
            });
        }

        public async Task<IEnumerable<MembershipLookupViewModel>> GetPlansLookupAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.Plans.GetAllAsync(cancellationToken: ct);
            return plans.Where(p => p.IsActive).Select(p => new MembershipLookupViewModel
            {
                Id = p.ID,
                Name = $"{p.Name} - {p.Price:N0} EGP ({p.DurationDays} Days)"
            });
        }


     
    }
}
