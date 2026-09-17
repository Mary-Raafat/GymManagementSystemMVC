using GymManagementSystem.BLL.Services;
using GymManagementSystem.BLL.ViewModels.Trainer;
using GymManagementSystem.DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers
{
    public class TrainerController(ITrainerService trainerService):Controller
    {
        public async Task<IActionResult> Index()
        {
            var allTrainers = await trainerService.GetAllAsync();
            return View(allTrainers);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTrainerViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var result = await trainerService.CreateAsync(viewModel);
            if (result.IsFailure)
            {
                ModelState.AddModelError(result.PropertyName ?? string.Empty, result.ErrorMessage ?? "An unexpected error occurred.");
                TempData["Error"] = "Cannot Create a trainer";

                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Trainer created successfully!";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var trainer = await trainerService.GetDetailsAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var trainer = await trainerService.GetForEditAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditTrainerViewModel viewModel, CancellationToken cancellationToken = default)
        {

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var result = await trainerService.UpdateAsync(viewModel, cancellationToken);
            if (result.IsFailure)
            {
                if (!string.IsNullOrEmpty(result.PropertyName))
                {
                    ModelState.AddModelError(result.PropertyName, result.ErrorMessage ?? "An unexpected error occurred.");
                }
                else
                {
                    TempData["Error"] = result.ErrorMessage ?? "Cannot Update a trainer";
                }
                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Trainer updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var trainer = await trainerService.GetDetailsAsync(id, cancellationToken);

            if (trainer == null) return NotFound();
            ViewBag.id = trainer.Id;
            return View(trainer);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            var result = await trainerService.DeleteAsync(id, cancellationToken);
            if (result.IsFailure)
            {
                TempData["Error"] = result.ErrorMessage ?? "Cannot delete trainer";
                return View();
            }

            TempData["SuccessMessage"] = "Trainer deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

    }
}
