using GymManagementSystem.Configurations;
using GymManagementSystem.DAL.Configurations;
using GymManagementSystem.DAL.Interceptors;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GymManagementSystem.Dbcontexts
{ 
    public class GymContext:DbContext
    {
       public GymContext(DbContextOptions<GymContext> options) : base(options)
        {
        }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new PlanConfiguration()); //object from the class configuration
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Member> Members => Set<Member>();
        public DbSet<Trainer> Trainers => Set<Trainer>();

        public DbSet<HealthRecord> HealthRecords { get; set; }
        public DbSet<Session> Sessions { get; set; }

        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Booking> Bookings { get; set; }

    }
}
