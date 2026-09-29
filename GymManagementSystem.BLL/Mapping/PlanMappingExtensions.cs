using GymManagementSystem.BLL.ViewModels.Plan;
using GymManagementSystem.Models;

namespace GymManagementSystem.BLL.Mapping
{
    public static class PlanMappingExtensions
    {
        // Plan → PlanViewModel (قائمة الخطط)
        public static PlanViewModel ToViewModel(this Plan plan)
        {
            return new PlanViewModel
            {
                Id = plan.ID,
                Name = plan.Name,
                Description = plan.Description,
                DurationDays = plan.DurationDays,
                Price = plan.Price,
                IsActive = plan.IsActive
            };
        }

        // Plan → PlanDetailsViewModel (تفاصيل الخطة)
        public static PlanDetailsViewModel ToDetailsViewModel(this Plan plan)
        {
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

        // Plan → EditPlanViewModel (للتعديل)
        public static EditPlanViewModel ToEditViewModel(this Plan plan)
        {
            return new EditPlanViewModel
            {
                Id = plan.ID,
                Name = plan.Name,
                Description = plan.Description,
                DurationDays = plan.DurationDays,
                Price = plan.Price
            };
        }
    }
}
