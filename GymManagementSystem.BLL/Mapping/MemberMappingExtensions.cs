using GymManagementSystem.BLL.ViewModels.Members;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Models.ValueOfObjects;

namespace GymManagementSystem.BLL.Mapping
{
    public static class MemberMappingExtensions
    {
        // Member → MemberViewModel (قائمة الأعضاء)
        public static MemberViewModel ToViewModel(this Member member)
        {
            return new MemberViewModel
            {
                Id = member.ID,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                PhotoUrl = member.Photo,
                JoinDate = DateOnly.FromDateTime(member.JoinDate),
                Gender = member.Gender.ToString()
            };
        }

        // Member → MemberDetailsViewModel (تفاصيل العضو مع الاشتراك)
        public static MemberDetailsViewModel ToDetailsViewModel(this Member member, Membership? activeMembership)
        {
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

        // Member → HealthRecordDetailsViewModel (تفاصيل السجل الصحي)
        public static HealthRecordDetailsViewModel ToHealthRecordDetailsViewModel(this HealthRecord healthRecord)
        {
            return new HealthRecordDetailsViewModel
            {
                BloodType = healthRecord.BloodType.ToString(),
                Height = healthRecord.Height,
                Weight = healthRecord.Weight,
                Notes = healthRecord.Notes ?? "-"
            };
        }

        // Member → EditMemberViewModel (للتعديل)
        public static EditMemberViewModel ToEditViewModel(this Member member)
        {
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

        // CreateMemberViewModel → Member (إنشاء عضو جديد)
        public static Member ToEntity(this CreateMemberViewModel viewModel, string normalizedName, string normalizedEmail, string normalizedPhone)
        {
            return new Member
            {
                Name = normalizedName,
                Email = normalizedEmail,
                Phone = normalizedPhone,
                DateOfBirth = viewModel.DateOfBirth,
                JoinDate = DateTime.UtcNow,
                Gender = viewModel.Gender,
                Address = new Address
                {
                    City = viewModel.City.Trim(),
                    Street = viewModel.Street.Trim(),
                    BuildingNumber = viewModel.BuildingNumber,
                },
                HealthRecord = new HealthRecord
                {
                    BloodType = viewModel.HealthRecordViewModel.BloodType,
                    Height = viewModel.HealthRecordViewModel.Height,
                    Weight = viewModel.HealthRecordViewModel.Weight,
                    Notes = viewModel.HealthRecordViewModel.Note?.Trim()
                }
            };
        }
    }
}
