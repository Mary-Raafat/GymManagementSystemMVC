using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories;
using GymManagementSystem.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable, IDisposable
    {
        IMemberRepository Members { get; }
        ITrainerRepo Trainers { get; }
        IPlanRepository Plans { get; }
        ISessionRepo Sessions { get; }
        IMembershipRepo Memberships { get; }
        IBookingRepo Bookings { get; }
        IGenericRepo<Category> Categories { get; }
        IGenericRepo<HealthRecord> HealthRecords { get; }

        IGenericRepo<TEntity> Repository<TEntity>() where TEntity : BaseEntity;

        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}
