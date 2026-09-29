using GymManagementSystem.DAL.Implementation;
using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.Dbcontexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Repositories
{
    public class SessionRepo(GymContext context) : GenericRepo<Session>(context), ISessionRepo
    {
        private readonly GymContext _context = context;
        //تتأكد من عدم وجود جلسة أخرى لنفس المدرب
        //(s.TrainerId == trainerId) تتقاطع في الوقت مع الميعاد الجديد باستخدام الشرط:
        public Task<bool> HasTrainerConflictAsync(int trainerId, DateTime startDate, DateTime endDate, int? excludeSessionId = null, CancellationToken ct = default)
        {
            return _context.Sessions
                .AnyAsync(s => s.TrainerId == trainerId
                            && (excludeSessionId == null || s.ID != excludeSessionId)
                            && s.StartDate < endDate
                            && s.EndDate > startDate, ct);
        }
    }
}
