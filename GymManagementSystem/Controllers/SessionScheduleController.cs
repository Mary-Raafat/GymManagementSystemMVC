using GymManagementSystem.BLL.Services;
using GymManagementSystem.BLL.ViewModels.Membership;
using GymManagementSystem.BLL.ViewModels.SessionSchedule;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem.PL.Controllers
{
    public class SessionScheduleController(ISessionScheduleService scheduleService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var schedule = await scheduleService.GetUpcomingScheduleAsync();
            return View(schedule);
        }

        [HttpGet]
        public async Task<IActionResult> Book(int sessionId)
        {
            var model = await scheduleService.GetForBookingAsync(sessionId);
            if (model == null)
            {
                return NotFound();
            }

            await PopulateEligibleMembersAsync(sessionId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(BookSessionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateEligibleMembersAsync(viewModel.SessionId);
                return View(viewModel);
            }

            var result = await scheduleService.BookMemberAsync(viewModel);
            if (result.IsFailure)
            {
                ModelState.AddModelError(result.PropertyName ?? string.Empty, result.ErrorMessage ?? "An unexpected error occurred.");
                TempData["Error"] = result.ErrorMessage ?? "Cannot complete booking";
                await PopulateEligibleMembersAsync(viewModel.SessionId);
                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Member booked successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Attendance(int sessionId)
        {
            var model = await scheduleService.GetSessionBookingsAsync(sessionId);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAttendance(int bookingId, int sessionId)
        {
            var result = await scheduleService.ToggleAttendanceAsync(bookingId);
            if (result.IsFailure)
            {
                TempData["Error"] = result.ErrorMessage ?? "Cannot update attendance.";
            }
            else
            {
                TempData["SuccessMessage"] = "Attendance updated successfully!";
            }

            return RedirectToAction(nameof(Attendance), new { sessionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int bookingId, int sessionId)
        {
            var result = await scheduleService.CancelBookingAsync(bookingId);
            if (result.IsFailure)
            {
                TempData["Error"] = result.ErrorMessage ?? "Cannot cancel booking.";
            }
            else
            {
                TempData["SuccessMessage"] = "Booking cancelled successfully!";
            }

            return RedirectToAction(nameof(Attendance), new { sessionId });
        }

        private async Task PopulateEligibleMembersAsync(int sessionId)
        {
            var members = await scheduleService.GetEligibleMembersLookupAsync(sessionId);
            ViewBag.Members = new SelectList(members, nameof(MembershipLookupViewModel.Id), nameof(MembershipLookupViewModel.Name));
        }
    }
}
