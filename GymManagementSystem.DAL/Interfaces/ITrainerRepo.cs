using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Interfaces
{
    public interface ITrainerRepo : IGenericRepo<Trainer>
    {
        public Task<bool> IsEmailTakenAsync(string email, int? excludeId = null, CancellationToken ct = default);
        public Task<bool> IsPhoneTakenAsync(string phone, int? excludeId = null, CancellationToken ct = default);

    }
}
