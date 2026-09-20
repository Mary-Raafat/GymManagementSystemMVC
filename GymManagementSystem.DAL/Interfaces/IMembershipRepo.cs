using GymManagementSystem.DAL.Models;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Interfaces
{
    public interface IMembershipRepo : IGenericRepo<Membership>
    {
        Task<bool> HasActiveMembershipAsync(int memberId, CancellationToken ct = default);
    }
}
