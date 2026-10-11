using GymManagementSystem.DAL.DataSeeding;
using GymManagementSystem.DAL.DataSeeding.Models;
using GymManagementSystem.Dbcontexts;
using GymManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Data
{
    public static class PlanSeeder
    {
        public static async Task SeedDataAsync(GymContext context)
        {
            // بنستخدم SeedJsonLoader عشان نقرأ ملف الـ JSON ونحوله لـ List من PlanSeedModel
            var seedModels = SeedJsonLoader.LoadFromJson<PlanSeedModel>("plan.json");

            var existingPlanNames = await context.Plans.Select(p => p.Name).ToListAsync();

            // بنحول كل PlanSeedModel لـ Plan entity عشان نضيفه لقاعدة البيانات
            var plans = seedModels.Where(s => !existingPlanNames.Contains(s.Name))
                .Select(s => new Plan
            {
                Name        = s.Name,
                Description = s.Description,
                DurationDays= s.DurationDays,
                Price       = s.Price,
                IsActive    = s.IsActive,
            }).ToList();

            if(plans.Count == 0)
                return;

            await context.Plans.AddRangeAsync(plans);
            await context.SaveChangesAsync();
        }
    }
}
