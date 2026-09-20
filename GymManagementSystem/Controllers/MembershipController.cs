using GymManagementSystem.BLL.Services;
using GymManagementSystem.BLL.ViewModels.Membership;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem.PL.Controllers
{
    public class MembershipController(IMembershipService membershipService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var memberships = await membershipService.GetAllAsync();
            return View(memberships);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMembershipViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(viewModel);
            }

            var result = await membershipService.CreateAsync(viewModel);
            if (result.IsFailure)
            {
                ModelState.AddModelError(result.PropertyName ?? string.Empty, result.ErrorMessage ?? "An unexpected error occurred.");
                TempData["Error"] = result.ErrorMessage ?? "Cannot create membership";
                await PopulateDropdownsAsync();
                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Membership created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await membershipService.CancelAsync(id);
            if (result.IsFailure)
            {
                TempData["Error"] = result.ErrorMessage ?? "Cannot cancel membership";
            }
            else
            {
                TempData["SuccessMessage"] = "Membership cancelled successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdownsAsync()
        {
            var members = await membershipService.GetMembersLookupAsync();
            var plans = await membershipService.GetPlansLookupAsync();

            ViewBag.Members = new SelectList(members, nameof(MembershipLookupViewModel.Id), nameof(MembershipLookupViewModel.Name));
            ViewBag.Plans = new SelectList(plans, nameof(MembershipLookupViewModel.Id), nameof(MembershipLookupViewModel.Name));
        }
    }
}
