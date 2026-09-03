using GymManagementSystem.DAL.Implementation;
using GymManagementSystem.DAL.Interceptors;
using GymManagementSystem.DAL.Repositories;
using GymManagementSystem.Dbcontexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.DAL.Interfaces;

namespace GymManagementSystem.DAL
{
    public static  class ServiceCollectionExtensionsDAL
    {
        public static IServiceCollection AddGymManagementSystemDAL(this IServiceCollection services,string connectionString)
        {
            services.AddSingleton<AuditColumnsInterceptor>();
            services.AddDbContext<GymContext>((serviceProvider, optionsBuilder) =>
            {
                var interceptor = serviceProvider.GetRequiredService<AuditColumnsInterceptor>();
                optionsBuilder.AddInterceptors(interceptor);
                optionsBuilder.UseSqlServer(connectionString);
            });

            //Dependency Injection
            services.AddScoped<IPlanRepository, PlanRepository>();
            services.AddScoped<IMemberRepository, MemberRepository>();

            services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            return services;
        }
    }
}
