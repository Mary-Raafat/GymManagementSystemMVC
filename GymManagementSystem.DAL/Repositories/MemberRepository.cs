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
       //عملتها بال GetByIdAsync
       // مفيده علشان ال GetDetailsAsync 
       
        //الايميل مكرر و لا لا 
        public Task<bool> IsEmailTakenAsync(string normalizedEmail, int? excludeId = null, CancellationToken ct = default)
            => _context.Users.AnyAsync(u => u.Email == normalizedEmail && (excludeId == null || u.ID != excludeId), ct);

        //الرقم مكرر و لا لا
        public Task<bool> IsPhoneTakenAsync(string phone, int? excludeId = null, CancellationToken ct = default)
            => _context.Users.AnyAsync(u => u.Phone == phone && (excludeId == null || u.ID != excludeId), ct);
    }
}
