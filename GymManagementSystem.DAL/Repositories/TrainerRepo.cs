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
    public class TrainerRepo(GymContext context) : GenericRepo<Trainer>(context), ITrainerRepo
    {
        private readonly GymContext _context = context;

        public Task<bool> IsEmailTakenAsync(string email, int? excludeId = null, CancellationToken ct = default)
            => _context.Users.AnyAsync(u => u.Email == email && (excludeId == null || u.ID != excludeId), ct);

        public Task<bool> IsPhoneTakenAsync(string phone, int? excludeId = null, CancellationToken ct = default)
            => _context.Users.AnyAsync(u => u.Phone == phone && (excludeId == null || u.ID != excludeId), ct);
    }
}
