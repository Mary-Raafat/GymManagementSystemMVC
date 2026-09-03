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
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {

            builder.Property(b => b.IsAttended)
                   .IsRequired()
                   .HasDefaultValue(false);

 
            builder.HasIndex(b => new { b.MemberId, b.SessionId })
                   .IsUnique().HasFilter("[IsDeleted]=0"); // طبق ال unique index حتى علي اللي مش متشالوش بال soft delete
            // يعني ميبقاش فيه حاجه متشاله اصلا و تيجي تقولي لا معلش هي unique مش هينفع اعمل زيها

            // Member (One Member -> Many Bookings)
            builder.HasOne(b => b.Member)
                   .WithMany(m => m.Bookings)
                   .HasForeignKey(b => b.MemberId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Session (One Session -> Many Bookings)
            builder.HasOne(b => b.Session)
                   .WithMany(s => s.Bookings)
                   .HasForeignKey(b => b.SessionId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Booking_Date",
                    "[Date] <= GETDATE()");
            });


            builder.HasQueryFilter(b => !b.IsDeleted);

        }
    }
}
