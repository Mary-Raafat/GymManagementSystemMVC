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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasDiscriminator<string>("UserType")
                   .HasValue<Member>("Member")
                   .HasValue<Trainer>("Trainer");

            builder.Property(t => t.Name).
                HasMaxLength(30);

            builder.Property(t => t.Email).
                HasMaxLength(100);

            builder.Property(t => t.Phone).
               HasMaxLength(100);

            builder.OwnsOne(x => x.Address, t =>
            {
                t.Property(m=>m.Street).HasColumnName("Street").HasMaxLength(100); 
                t.Property(m=>m.City).HasColumnName("City").HasMaxLength(100); //Before : Address.City
                t.Property(m=>m.BuildingNumber).HasColumnName("BuildingNumber"); 
            });


            builder.HasIndex(t => t.Email).IsUnique();
            builder.HasIndex(t => t.Phone).IsUnique();

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_User_Phone",
                    "LEN([Phone]) = 11 AND (" +
                    "[Phone] LIKE '010%' OR " +
                    "[Phone] LIKE '011%' OR " +
                    "[Phone] LIKE '012%' OR " +
                    "[Phone] LIKE '015%')"
                );
            });
            builder.HasQueryFilter(t => !t.IsDeleted); // بيبين الناس اللي متشالتش بال soft delete



        }

        

    }
}
