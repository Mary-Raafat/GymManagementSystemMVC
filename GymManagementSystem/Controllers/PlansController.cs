using GymManagementSystem.BLL.Services;
using GymManagementSystem.BLL.ViewModels.Plan;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers
{
    public class PlansController(IPlanService planService) : Controller
    {
        // GET: /Plans/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var plans = await planService.GetAllAsync();
            return View(plans);
        }

        // GET: /Plans/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var plan = await planService.GetDetailsAsync(id);
            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        // POST: /Plans/Activate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            var result = await planService.ToggleStatusAsync(id);
            if (result.IsFailure)
            {
                TempData["Error"] = result.ErrorMessage ?? "Cannot update plan status.";
            }
            else
            {
                TempData["SuccessMessage"] = "Plan status updated successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Plans/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var plan = await planService.GetForEditAsync(id);
            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        // POST: /Plans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditPlanViewModel viewModel, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var result = await planService.UpdateAsync(viewModel, ct);
            if (result.IsFailure)
            {
                if (!string.IsNullOrEmpty(result.PropertyName))
                {
                    ModelState.AddModelError(result.PropertyName, result.ErrorMessage ?? "An unexpected error occurred.");
                }
                else
                {
                    TempData["Error"] = result.ErrorMessage ?? "Cannot update plan.";
                }

                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Plan updated successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
