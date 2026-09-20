using System;

namespace GymManagementSystem.BLL.ViewModels.Membership
{
    public class MembershipViewModel
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; } = default!;
        public string MemberPhone { get; set; } = default!;
        public int PlanId { get; set; }
        public string PlanName { get; set; } = default!;
        public decimal PlanPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string StartDateDisplay => StartDate.ToString("dd MMM yyyy");
        public string EndDateDisplay => EndDate.ToString("dd MMM yyyy");
        public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
        public string Status => DateTime.UtcNow < StartDate ? "Upcoming" : (DateTime.UtcNow <= EndDate ? "Active" : "Expired");
        public int DaysRemaining => Math.Max(0, (int)(EndDate - DateTime.UtcNow).TotalDays);
    }
}
