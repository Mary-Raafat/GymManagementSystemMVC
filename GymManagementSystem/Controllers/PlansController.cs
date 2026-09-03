using GymManagementSystem.DAL.Repositories;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using GymManagementSystem.DAL.Interfaces;

namespace GymManagementSystem.Controllers
{
    public class PlansController : Controller
    {
        private readonly IPlanRepository _planRepository;

        public PlansController(IPlanRepository planrepo)
        {
            _planRepository = planrepo;   
        }

        // GET: /Plans/
        // جلب كل الـ plans من قاعدة البيانات وإرسالها للـ view
        public async Task<IActionResult> Index()
        {
            var plans = await _planRepository.GetAllAsync();
            return View(plans); 
        }

        // GET: /Plans/Details/5
        // جلب خطة معينة بناءً على الـ ID
        public async Task<IActionResult> Details(int? id) 
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var plan = await _planRepository.GetByIdAsync(id.Value);

            if (plan == null)
            {
                return RedirectToAction(nameof(Index)); 
            }

            return View(plan);
        }

        // POST: /Plans/Activate/5
        // تفعيل أو تعطيل الخطة
        [HttpPost]
        public async Task<IActionResult> Activate(int id)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            if (plan != null)
            {
                plan.IsActive = !plan.IsActive;
                _planRepository.Update(plan);
                await _planRepository.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Plans/Edit/5
        // عرض صفحة تعديل الخطة
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var plan = await _planRepository.GetByIdAsync(id.Value);
            if (plan == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        // POST: /Plans/Edit/5
        // حفظ تعديلات الخطة
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Plan plan)
        {
            if (id != plan.ID)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _planRepository.Update(plan);
                await _planRepository.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
    }
}


