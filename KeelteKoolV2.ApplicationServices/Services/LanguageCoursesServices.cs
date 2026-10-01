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
            if (dto == null)
            {
                return null;
            }
            LanguageCourse domain = new LanguageCourse
            {
                Id = Guid.NewGuid(),
                Nimetus = dto.Nimetus,
                Keel = dto.Keel,
                Tase = dto.Tase,
                Kirjeldus = dto.Kirjeldus,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };
            await _context.LanguageCourses.AddAsync(domain);
            await _context.SaveChangesAsync();
            return domain;
        }

        public async Task<LanguageCourse?> DetailAsync(Guid id)
        {
            LanguageCourse? domain = await _context.LanguageCourses.FirstOrDefaultAsync(x => x.Id == id);
            return domain;
        }

        public async Task<LanguageCourse?> Update(LanguageCourseDTO dto)
        {
            if (dto == null || dto.Id == null)
            {
                return null;
            }
            LanguageCourse? domain = await _context.LanguageCourses.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (domain == null)
            {
                return null;
            }
            domain.Nimetus = dto.Nimetus;
            domain.Keel = dto.Keel;
            domain.Tase = dto.Tase;
            domain.Kirjeldus = dto.Kirjeldus;
            domain.ModifiedAt = DateTime.UtcNow; // CreatedAt jääb samaks
            domain.ModifiedBy = dto.ModifiedBy;
            await _context.SaveChangesAsync();
            return domain;
        }

        public async Task<LanguageCourse?> Delete(Guid id)
        {
            LanguageCourse? domain = await _context.LanguageCourses.FirstOrDefaultAsync(x => x.Id == id);
            if (domain == null)
            {
                return null;
            }
            _context.LanguageCourses.Remove(domain);
            await _context.SaveChangesAsync();
            return domain;
        }
    }
}
