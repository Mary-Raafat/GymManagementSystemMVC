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
    public class HealthRecordConfiguration : IEntityTypeConfiguration<HealthRecord>
    {
        public void Configure(EntityTypeBuilder<HealthRecord> builder)
        {


            builder.Property(hr => hr.Weight)
                   .HasPrecision(5, 2)
                   .IsRequired();

            builder.Property(hr => hr.Height)
                   .HasPrecision(5, 2)
                   .IsRequired();

            builder.Property(hr => hr.BloodType)
                   .HasConversion<string>().HasMaxLength(100)
                   .IsRequired();

            //builder.HasOne(hr => hr.Member)
            //       .WithOne(m => m.HealthRecord)
            //       .HasForeignKey<HealthRecord>(hr => hr.MemberId)
            //       .OnDelete(DeleteBehavior.Cascade);
            //هيفهم لوحده 
            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_HealthRecord_Weight",
                    "[Weight] > 0");

                t.HasCheckConstraint(
                    "CK_HealthRecord_Height",
                    "[Height] > 0");
            });
        }

    }
    }
