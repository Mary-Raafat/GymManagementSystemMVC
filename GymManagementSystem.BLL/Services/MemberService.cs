using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Members;
using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Models.ValueOfObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public class MemberService(IUnitOfWork unitOfWork) : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IEnumerable<MemberViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.Members.GetAllAsync(cancellationToken: ct);
            return members.Select(m => new MemberViewModel
            {
                Id = m.ID,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                PhotoUrl = m.Photo,
                JoinDate = DateOnly.FromDateTime(m.JoinDate),
                Gender = m.Gender.ToString()
            });
        }
        public async Task<Result> CreateAsync(CreateMemberViewModel viewModel, CancellationToken ct = default)
        {
            var email = viewModel.Email.Trim().ToLowerInvariant();
            var phone = viewModel.Phone.Trim();
            var name = viewModel.Name.Trim();

            if (await _unitOfWork.Members.IsEmailTakenAsync(email, ct: ct))
            {
                return Result.Failure("Email already exists.", nameof(CreateMemberViewModel.Email));
            }

            if (await _unitOfWork.Members.IsPhoneTakenAsync(phone, ct: ct))
            {
                return Result.Failure("Phone number already exists.", nameof(CreateMemberViewModel.Phone));
            }
            var member = new Member // ربط ال member بال view model
            {
                Name = name,
                Email = email,
                Phone = phone,
                DateOfBirth = viewModel.DateOfBirth,
                JoinDate = DateTime.UtcNow,
                Gender = viewModel.Gender,
                Address = new Address
                {
                    City = viewModel.City.Trim(),
                    Street = viewModel.Street.Trim(),
                    BuildingNumber = viewModel.BuildingNumber,
                },

                HealthRecord = new HealthRecord // ربط ال health record 
                {
                    BloodType = viewModel.HealthRecordViewModel.BloodType,
                    Height = viewModel.HealthRecordViewModel.Height,
                    Weight = viewModel.HealthRecordViewModel.Weight,
                    Notes = viewModel.HealthRecordViewModel.Note?.Trim()
                }
            };

            await _unitOfWork.Members.AddAsync(member, ct);
            var rowsAffected = await _unitOfWork.CompleteAsync(ct);
            if (rowsAffected == 0)
            {
                return Result.Failure("Failed to add member.");
            }

            return Result.Success();
        }

        public async Task<MemberDetailsViewModel?> GetDetailsAsync(int id, CancellationToken cancellationToken = default)
        {

            //عشان يعرف يجيب ال plans 
            var member = await _unitOfWork.Members.GetByIdAsync(
                id: id,
                include: query => query.Include(m => m.Memberships).ThenInclude(ms => ms.Plan),
                trackChanges: false,
                cancellationToken: cancellationToken);

            if (member == null) return null;

            var today = DateTime.Today;
            Membership? activeMembership = member.Memberships.FirstOrDefault(m => m.EndDate >= today);

            return new MemberDetailsViewModel
            {
                Id = member.ID,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                PhotoUrl = member.Photo,
                Gender = member.Gender.ToString(),
                DateOfBirth = member.DateOfBirth.ToString("yyyy-MM-dd"),
                Address = $"{member.Address?.Street}, {member.Address?.City}, {member.Address?.BuildingNumber}",
                PlanName = activeMembership?.Plan?.Name ?? "No Active Plan",
                MemberShipStartDate = activeMembership?.StartDate.ToShortDateString() ?? "-",
                MemberShipEndDate = activeMembership?.EndDate.ToShortDateString() ?? "-",

            };
           
        }

        public async Task<HealthRecordDetailsViewModel?> GetHealthRecordDetailsAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(
                id: memberId,
                trackChanges: false,
                includes: [m => m.HealthRecord],
                cancellationToken: ct);

            if (member?.HealthRecord == null) return null;

            return new HealthRecordDetailsViewModel
            {
                BloodType = member.HealthRecord.BloodType.ToString(),
                Height = member.HealthRecord.Height,
                Weight = member.HealthRecord.Weight,
                Notes = member.HealthRecord.Notes ?? "-"
            };
        }

        public async Task<EditMemberViewModel?> GetForEditAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(id: id, trackChanges: false, cancellationToken: ct);
            if (member == null) return null;

            return new EditMemberViewModel
            {
                Id = member.ID,
                Name = member.Name,
                PhotoUrl = member.Photo,
                Email = member.Email,
                Phone = member.Phone,
                City = member.Address?.City ?? string.Empty,
                Street = member.Address?.Street ?? string.Empty,
                BuildingNumber = member.Address?.BuildingNumber ?? 0,
            };
        }

        public async Task<Result> UpdateAsync(EditMemberViewModel viewModel, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(viewModel.Id, cancellationToken: cancellationToken);
            if (member == null) return Result.Failure("Member Not Found", nameof(viewModel.Id));

            // يتأكد هل البيانات اتغيرت ولالا
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
                return Result.Failure("No changes were made.");
            }

            if (isEmailChanged && await _unitOfWork.Members.IsEmailTakenAsync(normalizedEmail, excludeId: viewModel.Id, ct: cancellationToken))
            {
                return Result.Failure("Email already exists.", nameof(viewModel.Email));
            }

            if(isPhoneChanged && await _unitOfWork.Members.IsPhoneTakenAsync(normalizedPhone, excludeId: viewModel.Id, ct: cancellationToken))
            {
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
                return Result.Failure("Failed to update member.");
            }

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(
                id,
                include: query => query.Include(m => m.HealthRecord)
                                       .Include(m => m.Bookings).ThenInclude(b => b.Session),
                cancellationToken: cancellationToken);

            if (member == null)
            {
                return Result.Failure("Member not found.", nameof(id));
            }

            bool hasActiveBookings = member.Bookings.Any(b => b.Session != null && b.Session.EndDate >= DateTime.UtcNow);
            if (hasActiveBookings)
            {
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
                return Result.Failure("Failed to delete member.");
            }

            return Result.Success();
        }
    }
}
