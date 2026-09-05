using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.Dbcontexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Implementation
{
    public class MemberRepository(GymContext context) : GenericRepo<Member>(context), IMemberRepository
    {


        private readonly GymContext _context = context;


       //تجيب ال memberships من ال member مع ال plan 
       // مفيده علشان ال GetDetailsAsync 
        public Task<Member?> GetWithMembershipsAsync(int id, CancellationToken ct = default)
        {
         return _context.Set<Member>()
                .AsNoTracking()
                .Include(m => m.Memberships)
                .ThenInclude(ms => ms.Plan)
                .FirstOrDefaultAsync(m => m.ID == id, ct);
        }

        //الايميل مكرر و لا لا 
        public Task<bool> IsEmailTakenAsync(string normalizedEmail, CancellationToken ct = default)
       => _context.Set<Member>().AnyAsync(m => m.Email == normalizedEmail, ct);

        //الرقم مكرر و لا لا
        public Task<bool> IsPhoneTakenAsync(string phone, CancellationToken ct = default)
        => _context.Set<Member>().AnyAsync(m => m.Phone == phone, ct);
    }
}
