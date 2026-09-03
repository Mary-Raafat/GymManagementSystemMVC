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
    public static  class CategorySeeder
    {
        public static async Task SeedCategories(GymContext context)
        {
            if (await context.Categories.AnyAsync())
                return;

            var categories = new List<Category>
            {
                new Category { Name = "Strength" },
                new Category { Name = "Cardio" },
                new Category { Name = "Yoga" },
                new Category { Name = "CrossFit" },
                new Category { Name = "Bodybuilding" },
                new Category { Name = "Weight Loss" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }
    }
}
