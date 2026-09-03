using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // مينفعش يبقى nvarchar(max) عشان ده هيبقى فيه مشاكل في الاداء

            builder.Property(c => c.Name).HasMaxLength(50);
            builder.HasQueryFilter(x => !x.IsDeleted);

        }
    }
}
