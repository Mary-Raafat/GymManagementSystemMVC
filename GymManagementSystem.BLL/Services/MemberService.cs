using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Mapping;
using GymManagementSystem.BLL.ViewModels.Members;
using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Models.ValueOfObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public class MemberService(IUnitOfWork unitOfWork, ILogger<MemberService> logger) : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<MemberService> _logger = logger;

        public async Task<IEnumerable<MemberViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all members.");
            var members = await _unitOfWork.Members.GetAllAsync(cancellationToken: ct);
            return members.Select(m => m.ToViewModel());
        }
        public async Task<Result> CreateAsync(CreateMemberViewModel viewModel, CancellationToken ct = default)
        {
            var email = viewModel.Email.Trim().ToLowerInvariant();
            var phone = viewModel.Phone.Trim();
            var name = viewModel.Name.Trim();

            _logger.LogInformation("Attempting to create a new member with email: {Email}", email);

            if (await _unitOfWork.Members.IsEmailTakenAsync(email, ct: ct))
            {
                _logger.LogWarning("Member creation failed: Email {Email} already exists.", email);
                return Result.Failure("Email already exists.", nameof(CreateMemberViewModel.Email));
            }

            if (await _unitOfWork.Members.IsPhoneTakenAsync(phone, ct: ct))
            {
                _logger.LogWarning("Member creation failed: Phone {Phone} already exists.", phone);
                return Result.Failure("Phone number already exists.", nameof(CreateMemberViewModel.Phone));
            }
            var member = viewModel.ToEntity(name, email, phone); // ربط ال member بال view model

            await _unitOfWork.Members.AddAsync(member, ct);
            var rowsAffected = await _unitOfWork.CompleteAsync(ct);
            if (rowsAffected == 0)
            {
                _logger.LogError("Failed to persist new member {Email} in the database.", email);
                return Result.Failure("Failed to add member.");
            }

            _logger.LogInformation("Member created successfully with ID: {MemberId}", member.ID);
            return Result.Success();
        }

        public async Task<MemberDetailsViewModel?> GetDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving details for member ID: {MemberId}", id);

            //عشان يعرف يجيب ال plans 
            var member = await _unitOfWork.Members.GetByIdAsync(
                id: id,
                include: query => query.Include(m => m.Memberships).ThenInclude(ms => ms.Plan),
                trackChanges: false,
                cancellationToken: cancellationToken);

            if (member == null)
            {
                _logger.LogWarning("Member with ID {MemberId} not found.", id);
                return null;
            }

            var today = DateTime.Today;
            Membership? activeMembership = member.Memberships.FirstOrDefault(m => m.EndDate >= today);

            return member.ToDetailsViewModel(activeMembership);
           
        }

        public async Task<HealthRecordDetailsViewModel?> GetHealthRecordDetailsAsync(int memberId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving health record for member ID: {MemberId}", memberId);

            var member = await _unitOfWork.Members.GetByIdAsync(
                id: memberId,
                trackChanges: false,
                includes: [m => m.HealthRecord],
                cancellationToken: ct);

            if (member?.HealthRecord == null)
            {
                _logger.LogWarning("Health record for member ID {MemberId} not found.", memberId);
                return null;
            }

            return member.HealthRecord.ToHealthRecordDetailsViewModel();
        }

        public async Task<EditMemberViewModel?> GetForEditAsync(int id, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving edit data for member ID: {MemberId}", id);

            var member = await _unitOfWork.Members.GetByIdAsync(id: id, trackChanges: false, cancellationToken: ct);
            if (member == null)
            {
                _logger.LogWarning("Member with ID {MemberId} not found for editing.", id);
                return null;
            }

            return member.ToEditViewModel();
        }

        public async Task<Result> UpdateAsync(EditMemberViewModel viewModel, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to update member ID: {MemberId}", viewModel.Id);

            var member = await _unitOfWork.Members.GetByIdAsync(viewModel.Id, cancellationToken: cancellationToken);
            if (member == null)
            {
                _logger.LogWarning("Member update failed: Member ID {MemberId} not found.", viewModel.Id);
                return Result.Failure("Member Not Found", nameof(viewModel.Id));
            }

            var normalizedEmail = viewModel.Email.Trim().ToLowerInvariant();
            var normalizedPhone = viewModel.Phone.Trim();
            var normalizedCity = viewModel.City.Trim();
            var normalizedStreet = viewModel.Street.Trim();

            bool isEmailChanged = member.Email != normalizedEmail;
            bool isPhoneChanged = member.Phone != normalizedPhone;
            bool isAddressChanged = member.Address == null ||
                                   member.Address.City != normalizedCity ||
                                   member.Address.Street != normalizedStreet ||
                                   member.Address.BuildingNumber != viewModel.BuildingNumber;

            if (!isEmailChanged && !isPhoneChanged && !isAddressChanged)
            {
                _logger.LogInformation("No changes detected for member ID: {MemberId}", viewModel.Id);
                return Result.Failure("No changes were made.");
            }

            if (isEmailChanged && await _unitOfWork.Members.IsEmailTakenAsync(normalizedEmail, excludeId: viewModel.Id, ct: cancellationToken))
            {
                _logger.LogWarning("Member update failed: Email {Email} is already taken.", normalizedEmail);
                return Result.Failure("Email already exists.", nameof(viewModel.Email));
            }

            if (isPhoneChanged && await _unitOfWork.Members.IsPhoneTakenAsync(normalizedPhone, excludeId: viewModel.Id, ct: cancellationToken))
            {
                _logger.LogWarning("Member update failed: Phone number {Phone} is already taken.", normalizedPhone);
                return Result.Failure("Phone number already exists.", nameof(viewModel.Phone));
            }

            member.Email = normalizedEmail;
            member.Phone = normalizedPhone;
            member.Address = new Address
            {
                City = normalizedCity,
                Street = normalizedStreet,
                BuildingNumber = viewModel.BuildingNumber
            };

            _unitOfWork.Members.Update(member);
            var rowsAffected = await _unitOfWork.CompleteAsync(cancellationToken);
            if (rowsAffected == 0)
            {
                _logger.LogError("Failed to persist updates for member ID: {MemberId}", viewModel.Id);
                return Result.Failure("Failed to update member.");
            }

            _logger.LogInformation("Member ID: {MemberId} updated successfully.", viewModel.Id);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete member ID: {MemberId}", id);

            var member = await _unitOfWork.Members.GetByIdAsync(
                id,
                include: query => query.Include(m => m.HealthRecord)
                                       .Include(m => m.Bookings).ThenInclude(b => b.Session),
                cancellationToken: cancellationToken);

            if (member == null)
            {
                _logger.LogWarning("Member deletion failed: Member ID {MemberId} not found.", id);
                return Result.Failure("Member not found.", nameof(id));
            }

            bool hasActiveBookings = member.Bookings.Any(b => b.Session != null && b.Session.EndDate >= DateTime.UtcNow);
            if (hasActiveBookings)
            {
                _logger.LogWarning("Member deletion rejected: Member ID {MemberId} has active bookings.", id);
                return Result.Failure("Cannot delete member with active bookings.");
            }

            if (member.HealthRecord != null)
            {
                member.HealthRecord.IsDeleted = true;
                member.HealthRecord.DeletedAt = DateTime.UtcNow;
            }

            await _unitOfWork.Members.SoftDeleteAsync(member, cancellationToken);
            var rowsAffected = await _unitOfWork.CompleteAsync(cancellationToken);
            if (rowsAffected == 0)
            {
                _logger.LogError("Failed to soft delete member ID: {MemberId}", id);
                return Result.Failure("Failed to delete member.");
            }

            _logger.LogInformation("Member ID: {MemberId} deleted successfully.", id);
            return Result.Success();
        }
    }
}
