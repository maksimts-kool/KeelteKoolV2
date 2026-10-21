using KeelteKoolV2.Core.DTO;
using KeelteKoolV2.Core.ServiceInterface;
using KeelteKoolV2.Data;
using KeelteKoolV2.Models.Students;
using Microsoft.AspNetCore.Mvc;

namespace KeelteKoolV2.Controllers
{
    public class StudentsController : Controller
    {
        private readonly KeelteKoolV2Context _context;
        private readonly IStudentsServices _studentsServices;

        public StudentsController(KeelteKoolV2Context context, IStudentsServices studentsServices)
        {
            _context = context;
            _studentsServices = studentsServices;
        }

        public IActionResult Index()
        {
            var result = _context.Students
                .Select(x => new StudentViewModel
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                }).OrderBy(x => x.LastName).Take(20);
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            StudentViewModel vm = new();
            return View("CreateUpdate", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentViewModel vm)
        {
            if (vm == null || !ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }
            var dto = new StudentDTO()
            {
                Id = vm.Id,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                DateOfBirth = vm.DateOfBirth,
                UserID = vm.UserID
            };
            var result = await _studentsServices.Create(dto);
            if (result == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var student = await _studentsServices.DetailsAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            var vm = MapToViewModel(student);
            ViewData["ViewType"] = "details";
            return View("DetailsDelete", vm);
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var student = await _studentsServices.DetailsAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            var vm = MapToViewModel(student);
            return View("CreateUpdate", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(StudentViewModel vm)
        {
            if (vm == null || !ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }
            var dto = new StudentDTO()
            {
                Id = vm.Id,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                DateOfBirth = vm.DateOfBirth,
                UserID = vm.UserID,
                CreatedAt = vm.CreatedAt
            };
            var result = await _studentsServices.Update(dto);
            if (result == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var student = await _studentsServices.DetailsAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            var vm = MapToViewModel(student);
            ViewData["ViewType"] = "delete";
            return View("DetailsDelete", vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var result = await _studentsServices.Delete(id);
            if (result == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction(nameof(Index));
        }

        private StudentViewModel MapToViewModel(Core.Domain.Student student)
        {
            return new StudentViewModel
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth,
                UserID = student.UserID,
                CreatedAt = student.CreatedAt
            };
        }
    }
}
