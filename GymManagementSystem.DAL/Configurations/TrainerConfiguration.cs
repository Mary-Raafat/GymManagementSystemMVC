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
    public class TrainerConfiguration : IEntityTypeConfiguration<Trainer>
    {
        public  void Configure(EntityTypeBuilder<Trainer> builder)
        {


            // هنا هضيف اي حاجه تخص ال member عادي

            builder.Property(t => t.Speciality).HasConversion<string>().HasMaxLength(500);

        }
    }
}
