using GymManagementSystem.DAL.Implementation;
using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.Dbcontexts;
using GymManagementSystem.Models;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymContext _context;
        private readonly ConcurrentDictionary<Type, object> _repositories = new();

        private IMemberRepository? _members;
        private ITrainerRepo? _trainers;
        private IPlanRepository? _plans;
        private ISessionRepo? _sessions;
        private IMembershipRepo? _memberships;
        private IBookingRepo? _bookings;
        private IGenericRepo<Category>? _categories;
        private IGenericRepo<HealthRecord>? _healthRecords;

        public UnitOfWork(GymContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IMemberRepository Members => _members ??= new MemberRepository(_context);
        public ITrainerRepo Trainers => _trainers ??= new TrainerRepo(_context);
        public IPlanRepository Plans => _plans ??= new PlanRepository(_context);
        public ISessionRepo Sessions => _sessions ??= new SessionRepo(_context);
        public IMembershipRepo Memberships => _memberships ??= new MembershipRepo(_context);
        public IBookingRepo Bookings => _bookings ??= new BookingRepo(_context);
        public IGenericRepo<Category> Categories => _categories ??= new GenericRepo<Category>(_context);
        public IGenericRepo<HealthRecord> HealthRecords => _healthRecords ??= new GenericRepo<HealthRecord>(_context);

        public IGenericRepo<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            return (IGenericRepo<TEntity>)_repositories.GetOrAdd(
                typeof(TEntity),
                _ => new GenericRepo<TEntity>(_context));
        }

      

        public Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
