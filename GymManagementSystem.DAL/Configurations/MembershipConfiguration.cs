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
    public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {

            //builder.HasIndex(m => new 
            //{ 
            //    m.MemberId,
            //    m.PlanId ,
            //    m.StartDate
            //})
            //.IsUnique();


            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Membership_Dates",
                    "[EndDate] > [StartDate]");
            });

           

            builder.HasQueryFilter(m => !m.IsDeleted);










        }
    }
}
