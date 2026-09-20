using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Interfaces
{
    public interface ISessionRepo : IGenericRepo<Session>
    {
        Task<bool> HasTrainerConflictAsync(int trainerId, DateTime startDate, DateTime endDate, int? excludeSessionId = null, CancellationToken ct = default);
    }
}
