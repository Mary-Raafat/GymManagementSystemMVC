using GymManagementSystem.Dbcontexts;
using GymManagementSystem.Models;
using Microsoft.EntityFrameworkCore;


namespace GymManagementSystem.DAL.Data
{
    public static class PlanSeeder
    {
        public static  async Task SeedDataAsync(GymContext context)
        {

            if (await context.Plans.AnyAsync())
                return;

            var plans = new List<Plan>
            {
                new Plan
                {
                    Name = "Basic",
                    Description = "Access to gym equipment only.",
                    DurationDays = 30,
                    Price = 300,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Plan
                {
                    Name = "Standard",
                    Description = "Gym access with group classes.",
                    DurationDays = 90,
                    Price = 800,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Plan
                {
                    Name = "Premium",
                    Description = "Gym access with personal trainer.",
                    DurationDays = 180,
                    Price = 1500,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Plan
                {
                    Name = "VIP",
                    Description = "Unlimited access with all premium services.",
                    DurationDays = 365,
                    Price = 2500,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                }
            };

            await context.Plans.AddRangeAsync(plans);
            await context.SaveChangesAsync();
        }
    }


}
