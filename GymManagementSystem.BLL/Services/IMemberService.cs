using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public interface IMemberService
    {
        public Task<IEnumerable<MemberViewModel>> GetAllAsync(CancellationToken ct = default);
        public Task<Result> CreateAsync(CreateMemberViewModel viewModel, CancellationToken ct = default);

    }
}
