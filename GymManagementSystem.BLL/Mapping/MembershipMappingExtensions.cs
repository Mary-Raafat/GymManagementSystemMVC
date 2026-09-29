using GymManagementSystem.BLL.ViewModels.Membership;
using GymManagementSystem.DAL.Models;

namespace GymManagementSystem.BLL.Mapping
{
    public static class MembershipMappingExtensions
    {
        // Membership → MembershipViewModel (قائمة الاشتراكات)
        public static MembershipViewModel ToViewModel(this Membership membership)
        {
            return new MembershipViewModel
            {
                Id = membership.ID,
                MemberId = membership.MemberId,
                MemberName = membership.Member?.Name ?? "N/A",
                MemberPhone = membership.Member?.Phone ?? "N/A",
                PlanId = membership.PlanId,
                PlanName = membership.Plan?.Name ?? "N/A",
                PlanPrice = membership.Plan?.Price ?? 0,
                StartDate = membership.StartDate,
                EndDate = membership.EndDate
            };
        }

        // CreateMembershipViewModel → Membership (إنشاء اشتراك جديد)
        public static Membership ToEntity(this CreateMembershipViewModel viewModel, DateTime endDate)
        {
            return new Membership
            {
                MemberId = viewModel.MemberId,
                PlanId = viewModel.PlanId,
                StartDate = viewModel.StartDate,
                EndDate = endDate
            };
        }

        // Member → MembershipLookupViewModel (قائمة البحث عن الأعضاء)
        public static MembershipLookupViewModel ToMemberLookupViewModel(this Member member)
        {
            return new MembershipLookupViewModel
            {
                Id = member.ID,
                Name = $"{member.Name} ({member.Phone})"
            };
        }

        // Plan → MembershipLookupViewModel (قائمة البحث عن الخطط)
        public static MembershipLookupViewModel ToPlanLookupViewModel(this Models.Plan plan)
        {
            return new MembershipLookupViewModel
            {
                Id = plan.ID,
                Name = $"{plan.Name} - {plan.Price:N0} EGP ({plan.DurationDays} Days)"
            };
        }
    }
}
