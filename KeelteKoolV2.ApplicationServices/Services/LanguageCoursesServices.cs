using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.DTO;
using KeelteKoolV2.Core.ServiceInterface;
using KeelteKoolV2.Data;
using Microsoft.EntityFrameworkCore;

namespace KeelteKoolV2.ApplicationServices.Services
{
    public class LanguageCoursesServices : ILanguageCoursesServices
    {
        private readonly KeelteKoolV2Context _context;

        public LanguageCoursesServices(KeelteKoolV2Context context)
        {
            _context = context;
        }

        public async Task<List<LanguageCourse>> GetAllAsync()
        {
            return await _context.LanguageCourses.OrderBy(x => x.Nimetus).ToListAsync();
        }

        public async Task<LanguageCourse> Create(LanguageCourseDTO dto)
        {
            var course = new LanguageCourse
            {
                Id = Guid.NewGuid(),
                Nimetus = dto.Nimetus,
                Keel = dto.Keel,
                Tase = dto.Tase,
                Kirjeldus = dto.Kirjeldus,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                ModifiedBy = dto.ModifiedBy,
            };

            await _context.LanguageCourses.AddAsync(course);
            await _context.SaveChangesAsync();

            return course;
        }

        public async Task<LanguageCourse?> DetailAsync(Guid id)
        {
            return await _context.LanguageCourses.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<LanguageCourse?> Update(LanguageCourseDTO dto)
        {
            if (dto.Id == null)
            {
                return null;
            }

            var course = await _context.LanguageCourses.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (course == null)
            {
                return null;
            }

            course.Nimetus = dto.Nimetus;
            course.Keel = dto.Keel;
            course.Tase = dto.Tase;
            course.Kirjeldus = dto.Kirjeldus;
            course.ModifiedAt = DateTime.UtcNow; // CreatedAt jääb samaks
            course.ModifiedBy = dto.ModifiedBy;

            await _context.SaveChangesAsync();

            return course;
        }

        public async Task<LanguageCourse?> Delete(Guid id)
        {
            var course = await _context.LanguageCourses.FirstOrDefaultAsync(x => x.Id == id);
            if (course == null)
            {
                return null;
            }

            _context.LanguageCourses.Remove(course);
            await _context.SaveChangesAsync();

            return course;
        }
    }
}
