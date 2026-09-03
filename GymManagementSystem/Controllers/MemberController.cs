using GymManagementSystem.BLL.Services;
using GymManagementSystem.BLL.ViewModels;
using GymManagementSystem.BLL.ViewModels.Members;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers
{
    public class MemberController(IMemberService memberService) : Controller
    { 

        public async Task<IActionResult> Index()
        {
            var allMembers = await memberService.GetAllAsync();
            return View(allMembers);
        }

        [HttpGet]// عشان يعرض الفورم
        public async Task<IActionResult> Create() 
        {
            return View();
        }




        [HttpPost]
        public async Task<IActionResult> Create(CreateMemberViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var result = await memberService.CreateAsync(viewModel);
            if (result.IsFailure)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "An unexpected error occurred.");
                TempData["Error"] = "Cannot Create a member";

                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Member created successfully!";
            return RedirectToAction(nameof(Index));
        }

    }
}
