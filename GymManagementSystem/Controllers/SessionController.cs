using GymManagementSystem.BLL.Services;
using GymManagementSystem.BLL.ViewModels.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem.PL.Controllers
{
    public class SessionController(ISessionService sessionService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var allSessions = await sessionService.GetAllAsync();
            return View(allSessions);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSessionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(viewModel);
            }

            var result = await sessionService.CreateAsync(viewModel);
            if (result.IsFailure)
            {
                ModelState.AddModelError(result.PropertyName ?? string.Empty, result.ErrorMessage ?? "An unexpected error occurred.");
                TempData["Error"] = result.ErrorMessage ?? "Cannot Create a session";
                await PopulateDropdownsAsync();
                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Session created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var session = await sessionService.GetDetailsAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var session = await sessionService.GetForEditAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            await PopulateDropdownsAsync();
            return View(session);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditSessionViewModel viewModel, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(viewModel);
            }

            var result = await sessionService.UpdateAsync(viewModel, cancellationToken);
            if (result.IsFailure)
            {
                if (!string.IsNullOrEmpty(result.PropertyName))
                {
                    ModelState.AddModelError(result.PropertyName, result.ErrorMessage ?? "An unexpected error occurred.");
                }
                else
                {
                    TempData["Error"] = result.ErrorMessage ?? "Cannot Update a session";
                }

                await PopulateDropdownsAsync();
                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Session updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var session = await sessionService.GetDetailsAsync(id, cancellationToken);
            if (session == null)
            {
                return NotFound();
            }

            ViewBag.id = session.Id;
            return View(session);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken = default)
        {
            var result = await sessionService.DeleteAsync(id, cancellationToken);
            if (result.IsFailure)
            {
                TempData["Error"] = result.ErrorMessage ?? "Cannot delete session";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Session deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdownsAsync()
        {
            var trainers = await sessionService.GetTrainersLookupAsync();
            var categories = await sessionService.GetCategoriesLookupAsync();

            ViewBag.Trainers = new SelectList(trainers, nameof(SessionLookupViewModel.Id), nameof(SessionLookupViewModel.Name));
            ViewBag.Categories = new SelectList(categories, nameof(SessionLookupViewModel.Id), nameof(SessionLookupViewModel.Name));
        }
    }
}
