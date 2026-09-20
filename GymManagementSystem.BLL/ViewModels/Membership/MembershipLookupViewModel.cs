namespace GymManagementSystem.BLL.ViewModels.Membership
{
    public class MembershipLookupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? ExtraInfo { get; set; }
    }
}
