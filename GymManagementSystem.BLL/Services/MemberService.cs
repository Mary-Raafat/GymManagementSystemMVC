using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Members;
using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.DAL.Models.ValueOfObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public class MemberService(IMemberRepository memberRepository) : IMemberService
    {

        public async Task<IEnumerable<MemberViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var members = await memberRepository.GetAllAsync(cancellationToken: ct);
            var memberViewModels = members.Select(m => new MemberViewModel
            {
                Id = m.ID,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                PhotoUrl = m.Photo,
                JoinDate = DateOnly.FromDateTime(m.JoinDate),
                Gender = m.Gender.ToString()
            });

            return memberViewModels;
        }
        public async Task<Result> CreateAsync(CreateMemberViewModel viewModel, CancellationToken ct = default)
        {
            var email = viewModel.Email.Trim().ToLower();
            var phone = viewModel.Phone.Trim().ToLower();
            var name = viewModel.Name.Trim().ToLower();
            if (await memberRepository.ExistAsync(m => m.Email == email, ct))
            {
                return Result.Failure("Email already exists.", nameof(CreateMemberViewModel.Email));
            }

            if (await memberRepository.ExistAsync(m => m.Phone == phone, ct))
            {
                return Result.Failure("Phone number already exists.", nameof(CreateMemberViewModel.Phone));
            }
            var member = new Member // ربط ال member بال view model
            {
                Name = name,
                Email = email,
                Phone = phone,
                JoinDate = DateTime.UtcNow,
                Gender = viewModel.Gender,
                Address = new DAL.Models.ValueOfObjects.Address
                {
                    City = viewModel.City,
                    Street = viewModel.Street,
                    BuildingNumber = viewModel.BuildingNumber,
                },

                HealthRecord = new HealthRecord // ربط ال health record 
                {
                    BloodType = viewModel.HealthRecordViewModel.BloodType,
                    Height = viewModel.HealthRecordViewModel.Height,
                    Weight = viewModel.HealthRecordViewModel.Weight,
                }
            };
            await memberRepository.AddAsync(member, ct);
            var rowsAffected = await memberRepository.SaveChangesAsync(ct);
            if (rowsAffected == 0)
            {
                return Result.Failure("Failed to add member.");
            }

            return Result.Success();
        }

        public async Task<MemberDetailsViewModel?> GetDetailsAsync(int id, CancellationToken cancellationToken = default)
        {

            //عشان يعرف يجيب ال plans 
            var member = await memberRepository.GetWithMembershipsAsync(
                id: id,
                ct:cancellationToken);

            if (member == null) return null!;


            var today = DateTime.Today;
            Membership? activeMembership = member.Memberships.FirstOrDefault(m => m.EndDate >= today);

            return new MemberDetailsViewModel
            {
                Id = member.ID,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                PhotoUrl = member.Photo,
                Gender=member.Gender.ToString(),
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Address = $"{member.Address.Street}, {member.Address.City}, {member.Address.BuildingNumber}",
                PlanName = activeMembership?.Plan?.Name ?? "No Active Plan",
                MemberShipStartDate = activeMembership?.StartDate.ToShortDateString() ?? "-",
                MemberShipEndDate = activeMembership?.EndDate.ToShortDateString() ?? "-",

            };
           
        }

        public async Task<HealthRecordDetailsViewModel?> GetHealthRecordDetailsAsync(int memberId, CancellationToken ct = default)
        {
            var healthRecord = await memberRepository.GetByIdAsync(id: memberId, 
                trackChanges: false,
                includes: [m => m.HealthRecord],
                cancellationToken: ct);




            if (healthRecord == null) return null!;


            return new HealthRecordDetailsViewModel
            {
                BloodType = healthRecord.HealthRecord.BloodType.ToString(),
                Height = healthRecord.HealthRecord.Height,
                Weight = healthRecord.HealthRecord.Weight,
                Notes = healthRecord.HealthRecord.Notes
            };
        }

        public async Task<EditMemberViewModel?> GetForEditAsync(int id, CancellationToken ct = default)
        {
            var member=  await memberRepository.GetByIdAsync(id:id , cancellationToken: ct);
            if(member == null) return null!;

            return new EditMemberViewModel
            {

                Id = member.ID,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                City = member.Address.City,
                Street = member.Address.Street,
                BuildingNumber = member.Address.BuildingNumber,
            };
        }

        public async Task<Result> UpdateAsync(EditMemberViewModel viewModel, CancellationToken cancellationToken)
        {
            var member = await memberRepository.GetByIdAsync(viewModel.Id, cancellationToken: cancellationToken);
            if(member == null) return Result.Failure("Member Not Found", nameof(viewModel.Id));



            // يتأكد هل البيانات اتغيرت ولالا
            var normalizedEmail = viewModel.Email.Trim().ToLowerInvariant();
            var normalizedPhone = viewModel.Phone.Trim();
            var normalizedCity = viewModel.City.Trim();
            var normalizedStreet = viewModel.Street.Trim();

            bool isEmailChanged = member.Email != normalizedEmail;
            bool isPhoneChanged = member.Phone != normalizedPhone;
            bool isAddressChanged = member.Address.City != normalizedCity ||
                                   member.Address.Street != normalizedStreet ||
                                   member.Address.BuildingNumber != viewModel.BuildingNumber;

            if (!isEmailChanged && !isPhoneChanged && !isAddressChanged)
            {
                return Result.Failure("No changes were made.");
            }



            if(isEmailChanged && await memberRepository.IsEmailTakenAsync(normalizedEmail, excludeId: viewModel.Id, ct: cancellationToken))
            {
                return Result.Failure("Email already exists.", nameof(viewModel.Email));
            }

            if(isPhoneChanged && await memberRepository.IsPhoneTakenAsync(normalizedPhone, excludeId: viewModel.Id, ct: cancellationToken))
            {
                return Result.Failure("Phone number already exists.", nameof(viewModel.Phone));
            }

            member.Email = normalizedEmail;
            member.Phone = normalizedPhone;
            member.Address = new Address
            {
                City = viewModel.City.Trim(),
                Street = viewModel.Street.Trim(),
                BuildingNumber = viewModel.BuildingNumber
            };


            memberRepository.Update(member);
            await memberRepository.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }
    }
}

