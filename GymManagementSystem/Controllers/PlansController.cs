using GymManagementSystem.Dbcontexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Controllers
{
    public class PlansController : Controller
    {
        private GymContext _dbcontext;
        public PlansController()
        {
            _dbcontext = new GymContext();
        }
        // Get baseUrl/Plan/Index
        // Get /Plans/
        //عايز اكلم الداتا بيز اجيب منها كل ال plans 
        //2- ابعتها لل views
        // عايز اكلم الداتا بيز ؟  اعمل اوبجيكت من dbcontext class
        public async Task<IActionResult> Index()
        {
            var plans = await _dbcontext.Plans.ToListAsync();
            return View(plans); // بسأل هل هو static or dynamic => dynamic محتاج اديله الداتا 
            //  بيروح يدور علي فيو بنفس اسم الاكشن جوا فولدر ال views or shared 

        }


        //Get baseUrl/Plan/Details/1
        public async Task<IActionResult> Details(int id) // هيقدر ي  bind from route
        {

            var plans = await _dbcontext.Plans.FindAsync(id);

            if (plans == null)
            {

                return RedirectToAction(nameof(Index)); // iF id is null    
                                                        //  يوديني علي صفحه ال index (Homepage)
            }
            return View(plans);



        }
    }
}
