using KeelteKoolV2.Core.DTO;
using KeelteKoolV2.Core.ServiceInterface;
using KeelteKoolV2.Data;
using KeelteKoolV2.Models.LanguageCourses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeelteKoolV2.Controllers
{
    public class LanguageCoursesController : Controller
    {
        private readonly KeelteKoolV2Context _context;
        private readonly ILanguageCoursesServices _languageCoursesServices;

        public LanguageCoursesController(KeelteKoolV2Context context, ILanguageCoursesServices languageCoursesServices)
        {
            _context = context;
            _languageCoursesServices = languageCoursesServices;
        }
        public IActionResult Index()
        {
            ////gets everything
            //var result = _context.LanguageCourses.ToList();
            // get only some, with limited info
            var result = _context.LanguageCourses
                .Select(x => new LanguageCourseViewModel
                {
                    Id = x.Id,
                    Nimetus = x.Nimetus,
                    Keel = x.Keel,
                }).OrderBy(x => x.Keel).Take(20);
            return View(result);

        }

        [HttpGet]
        public IActionResult Create()
        {
            LanguageCourseViewModel vm = new();
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(LanguageCourseViewModel vm) 
        {
            //kontrollime et vm ei oleks null
            if (vm == null)
            {
                return RedirectToAction("Error", "Home");
            }
            //kontrollime et vmi modelstate on õige
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }
            //teeme uue DTO-objekti
            //asetame dtosse vmi andmed
            var dto = new LanguageCourseDTO() 
            {
                Id = vm.Id,
                Nimetus = vm.Nimetus,
                Keel = vm.Keel,
                Tase = vm.Tase,
                Kirjeldus = vm.Kirjeldus
            };
            //teostatakse päring teenusele
            var result = await _languageCoursesServices.Create(dto);
            //teenus peab objekti tagastama
            //kontrollime kas tagastatud objekt on null
            if (result == null)
            {
                //  kui on, suuname vealehele
                return RedirectToAction("Error", "Home");
            }
            else
            {
                //  kui ei, suuname tagasi indeksisse
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var course = await _languageCoursesServices.DetailsAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            var vm = new LanguageCourseViewModel
            {
                Id = course.Id,
                Nimetus = course.Nimetus,
                Keel = course.Keel,
                Tase = course.Tase,
                Kirjeldus = course.Kirjeldus
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var course = await _languageCoursesServices.DetailsAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            var vm = new LanguageCourseViewModel
            {
                Id = course.Id,
                Nimetus = course.Nimetus,
                Keel = course.Keel,
                Tase = course.Tase,
                Kirjeldus = course.Kirjeldus,
                CreatedAt = course.CreatedAt
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(LanguageCourseViewModel vm)
        {
            if (vm == null || !ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }
            var dto = new LanguageCourseDTO()
            {
                Id = vm.Id,
                Nimetus = vm.Nimetus,
                Keel = vm.Keel,
                Tase = vm.Tase,
                Kirjeldus = vm.Kirjeldus,
                CreatedAt = vm.CreatedAt
            };
            var result = await _languageCoursesServices.Update(dto);
            if (result == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var course = await _languageCoursesServices.DetailsAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            var vm = new LanguageCourseViewModel
            {
                Id = course.Id,
                Nimetus = course.Nimetus,
                Keel = course.Keel,
                Tase = course.Tase,
                Kirjeldus = course.Kirjeldus
            };
            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var result = await _languageCoursesServices.Delete(id);
            if (result == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
