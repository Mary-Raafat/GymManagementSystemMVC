using GymManagementSystem.DAL.Implementation; 
using GymManagementSystem.DAL.Interceptors;
using GymManagementSystem.DAL.Repositories;
using GymManagementSystem.Dbcontexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.BLL.Services; 

namespace GymManagementSystem.DAL
{
    public static  class ServiceCollectionExtensionsBLL
    {
        public static IServiceCollection AddGymManagementSystemBLL(this IServiceCollection services)
        {
            
            services.AddScoped<IMemberService, MemberService>();                
            return services;
        }
    }
}
