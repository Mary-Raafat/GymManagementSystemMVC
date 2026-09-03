using GymManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(p => p.Name)
                .HasColumnType("varchar")
                .HasMaxLength(50);

            builder.Property(p => p.Description).HasMaxLength(200);
            builder.Property(p => p.Price).HasPrecision(10, 2);// بحوله للحاجه الي انا عايزها لانه ال default precision (18,2)


            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");



            builder.HasQueryFilter(x => !x.IsDeleted);


            builder.ToTable(TB
                =>
            {
                TB.HasCheckConstraint("PlanDurationCheck", "DurationDays between 1 and 365"); // constraint name , constraint in sql 
            });

                }
    }
}
