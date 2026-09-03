using GymManagementSystem.Dbcontexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Data
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(GymContext dbcontext)
        {

            await PlanSeeder.SeedDataAsync(dbcontext);
            await CategorySeeder.SeedCategories(dbcontext);
            await UserSeeder.SeedUsers(dbcontext);
            await HealthRecordSeeder.SeedHealthRecords(dbcontext);

        }
    }
}
