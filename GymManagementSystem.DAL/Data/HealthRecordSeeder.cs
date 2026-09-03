using GymManagementSystem.DAL.Enums;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.Dbcontexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Data
{
    public static class HealthRecordSeeder
    {
        public static async Task SeedHealthRecords(GymContext context)
        {
            if (await context.HealthRecords.AnyAsync())
                return;

            var members = await context.Users
                .OfType<Member>()
                .ToListAsync();

            var healthRecords = new List<HealthRecord>
        {
            new HealthRecord
            {
                MemberId = members[0].ID,
                Weight = 75,
                Height = 178,
                BloodType = BloodType.APositive
            },
            new HealthRecord
            {
                MemberId = members[1].ID,
                Weight = 62,
                Height = 165,
                BloodType = BloodType.BPositive
            },
            new HealthRecord
            {
                MemberId = members[2].ID,
                Weight = 83,
                Height = 182,
                BloodType = BloodType.OPositive
            }
        };

            await context.HealthRecords.AddRangeAsync(healthRecords);
            await context.SaveChangesAsync();
        }
    }
}
