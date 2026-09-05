using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Interfaces
{
    public interface IMemberRepository:IGenericRepo<Member>
    {
        public Task<Member?>GetWithMembershipsAsync(int id , CancellationToken ct = default);
        public Task<bool> IsEmailTakenAsync(string normalizedEmail, int? excludeId = null, CancellationToken ct = default);
        public Task<bool> IsPhoneTakenAsync(string phone, int? excludeId = null, CancellationToken ct = default);

    }
}
