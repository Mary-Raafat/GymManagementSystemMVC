using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Configurations
{
    internal class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.Property(s => s.Description)
                              .IsRequired()
                              .HasMaxLength(500);

            builder.Property(s => s.Capacity)
                   .IsRequired();

            builder.Property(s => s.StartDate)
                   .IsRequired();

            builder.Property(s => s.EndDate)
                   .IsRequired();

          

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Session_Capacity",
                    "[Capacity] BETWEEN 1 AND 25");

                t.HasCheckConstraint(
                    "CK_Session_Dates",
                    "[EndDate] > [StartDate]");
            });

            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }
}
    
