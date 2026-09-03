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

    //عايز انفذ ال config بتاع ال user 
    // اخليه virtual هناك 
    // و هنا هخليه override
    public class MemberConfiguration :IEntityTypeConfiguration<Member>
    {
        public  void Configure(EntityTypeBuilder<Member> builder)
        {


            // هنا هضيف اي حاجه تخص ال member عادي

            builder.Property(m => m.Photo).HasMaxLength(500);

        }
    }
}
