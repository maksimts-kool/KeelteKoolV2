using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.DTO;
using KeelteKoolV2.Core.ServiceInterface;
using KeelteKoolV2.Models.LanguageCourses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeelteKoolV2.Controllers
{
    [Authorize]
    public class LanguageCoursesController : Controller
    {
        private readonly ILanguageCoursesServices _languageCoursesServices;

        public LanguageCoursesController(ILanguageCoursesServices languageCoursesServices)
        {
            _languageCoursesServices = languageCoursesServices;
        }
        // Sisukord:
        //
        // Nimekiri ja detailvaade (avalik)
        // Lisamine
        // Muutmine
        // Kustutamine

        /*     N I M E K I R I     J A     D E T A I L V A A D E     */

        /// <summary>
        /// Näitab kõiki keelekursusi
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var courses = await _languageCoursesServices.GetAllAsync();

            return View(courses.Select(ToViewModel).ToList());
        }

        /// <summary>
        /// Näitab ühe kursuse andmeid
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id)
        {
            var course = await _languageCoursesServices.DetailAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            return View(ToViewModel(course));
        }

        /*     L I S A M I N E     */

        [HttpGet]
        public IActionResult Create()
        {
            return View("CreateUpdate", new LanguageCourseViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(LanguageCourseViewModel vm)
        {
            if (vm == null)
            {
                return RedirectToAction("Error", "Home");
            }
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }

            var dto = new LanguageCourseDTO
            {
                Id = vm.Id,
                Nimetus = vm.Nimetus,
                Keel = vm.Keel,
                Tase = vm.Tase,
                Kirjeldus = vm.Kirjeldus,
            };

            var result = await _languageCoursesServices.Create(dto);

            if (result == null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        /*     M U U T M I N E     */

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var course = await _languageCoursesServices.DetailAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            return View("CreateUpdate", ToViewModel(course));
        }

        [HttpPost]
        public async Task<IActionResult> Update(LanguageCourseViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateUpdate", vm);
            }

            var course = await _languageCoursesServices.Update(ToDto(vm));
            if (course == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id = course.Id });
        }

        /*     K U S T U T A M I N E     */

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var course = await _languageCoursesServices.DetailAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            return View(ToViewModel(course));
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var course = await _languageCoursesServices.Delete(id);
            if (course == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        /* üleval tegevused, all abimeetodid */

        private static LanguageCourseViewModel ToViewModel(LanguageCourse course)
        {
            return new LanguageCourseViewModel
            {
                Id = course.Id,
                Nimetus = course.Nimetus,
                Keel = course.Keel,
                Tase = course.Tase,
                Kirjeldus = course.Kirjeldus,
            };
        }

        // CreatedAt ja ModifiedAt paneb service ise, ModifiedBy tuleb sisselogitud kasutajalt
        // (ViewModel neid ei sisalda, sest kasutaja ei tohi neid ise määrata)
        private LanguageCourseDTO ToDto(LanguageCourseViewModel vm)
        {
            return new LanguageCourseDTO
            {
                Id = vm.Id,
                Nimetus = vm.Nimetus,
                Keel = vm.Keel,
                Tase = vm.Tase,
                Kirjeldus = vm.Kirjeldus,
                ModifiedBy = User.Identity?.Name ?? string.Empty,
            };
        }
    }
}
