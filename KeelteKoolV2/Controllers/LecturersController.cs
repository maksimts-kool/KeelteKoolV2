using KeelteKoolV2.Core.DTO;
using KeelteKoolV2.Core.ServiceInterface;
using KeelteKoolV2.Data;
using KeelteKoolV2.Models.Lecturers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KeelteKoolV2.Controllers
{
    public class LecturersController : Controller
    {
        private readonly KeelteKoolV2Context _context;
        private readonly ILecturersServices _lecturersServices;
        private readonly IFileServices _fileServices;

        public LecturersController(KeelteKoolV2Context context, ILecturersServices lecturersServices, IFileServices fileServices)
        {
            _context = context;
            _lecturersServices = lecturersServices;
            _fileServices = fileServices;
        }

        public IActionResult Index()
        {
            var result = _context.Lecturers
                .Select(x => new LecturerViewModel
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                }).OrderBy(x => x.LastName).Take(20);
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            LecturerViewModel vm = new();
            return View("CreateUpdate", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LecturerViewModel vm)
        {
            if (vm == null || !ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }
            var dto = new LecturerDTO()
            {
                Id = vm.Id,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Qualifications = vm.Qualifications,
                UserID = vm.UserID,
                Files = vm.Files,
                Image = vm.Image.Select(x => new FileToDatabaseDTO
                {
                    ImageID = x.ImageID,
                    ImageTitle = x.ImageTitle,
                    ImageData = x.ImageData,
                    LecturerId = x.LecturerId
                }).ToArray()
            };
            var result = await _lecturersServices.Create(dto);
            if (result == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var lecturer = await _lecturersServices.DetailsAsync(id);
            if (lecturer == null)
            {
                return NotFound();
            }
            var vm = await MapToViewModel(lecturer);
            ViewData["ViewType"] = "details";
            return View("DetailsDelete", vm);
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var lecturer = await _lecturersServices.DetailsAsync(id);
            if (lecturer == null)
            {
                return NotFound();
            }
            var vm = await MapToViewModel(lecturer);
            return View("CreateUpdate", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(LecturerViewModel vm)
        {
            if (vm == null || !ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }
            var dto = new LecturerDTO()
            {
                Id = vm.Id,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Qualifications = vm.Qualifications,
                UserID = vm.UserID,
                Files = vm.Files,
                CreatedAt = vm.CreatedAt
            };
            var result = await _lecturersServices.Update(dto);
            if (result == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var lecturer = await _lecturersServices.DetailsAsync(id);
            if (lecturer == null)
            {
                return NotFound();
            }
            var vm = await MapToViewModel(lecturer);
            ViewData["ViewType"] = "delete";
            return View("DetailsDelete", vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var result = await _lecturersServices.Delete(id);
            if (result == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveImage(LecturerImageViewModel vm)
        {
            var dto = new FileToDatabaseDTO() { ImageID = vm.ImageID };
            var image = await _fileServices.RemoveImage(dto);
            if (image == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction(nameof(Update), new { id = image.LecturerId });
        }

        private async Task<LecturerViewModel> MapToViewModel(Core.Domain.Lecturer lecturer)
        {
            var images = await _context.Files
                .Where(x => x.LecturerId == lecturer.Id)
                .Select(y => new LecturerImageViewModel
                {
                    ImageID = y.ImageID,
                    ImageTitle = y.ImageTitle,
                    ImageData = y.ImageData,
                    LecturerId = y.LecturerId,
                    Image = string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(y.ImageData))
                }).ToArrayAsync();

            return new LecturerViewModel
            {
                Id = lecturer.Id,
                FirstName = lecturer.FirstName,
                LastName = lecturer.LastName,
                Qualifications = lecturer.Qualifications,
                UserID = lecturer.UserID,
                CreatedAt = lecturer.CreatedAt,
                Image = images.ToList()
            };
        }
    }
}
