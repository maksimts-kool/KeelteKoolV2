using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.DTO;
using KeelteKoolV2.Core.ServiceInterface;
using KeelteKoolV2.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeelteKoolV2.ApplicationServices.Services
{
    public class LanguageCoursesServices : ILanguageCoursesServices
    {
        private readonly KeelteKoolV2Context _context;

        public LanguageCoursesServices(KeelteKoolV2Context context)
        {
            _context = context;
        }

        public async Task<LanguageCourse> Create(LanguageCourseDTO dto)
        {
            //kontrollitakse kas dto omab mingeid andmeid
            if (dto == null) 
            {
                return null;
            }

            //tekitab uue andmebaasis istuva objekti vastava mudeli jörgi
            LanguageCourse domain = new LanguageCourse();

            //omistab andmed andmeedasikandeobjektist domeenimudelile
            //lahendades ära küsimused mis ei lahendatud kontrolleris, nagu näiteks id, createdat, modifiedat
            domain.Id = Guid.NewGuid();
            domain.Nimetus = "";
            domain.Keel = "";
            //if nimetus empty, return null
            if (dto.Nimetus.Length < 1)
            {
                return null;
            }
            if (dto.Keel.Length < 1)
            {
                return null;
            }
            domain.Kirjeldus = dto.Kirjeldus;
            domain.Tase = dto.Tase;
            domain.Keel = dto.Keel;
            domain.Nimetus = dto.Nimetus;
            domain.CreatedAt = DateTime.Now;
            domain.ModifiedAt = DateTime.Now;
            //later, require user id to be attached to "ModifiedBy" parameter, to know who modified last.

            //Todo: check if db addition succeeded, if yes, return object, if not, null
            //teostatakse andmebaasi lisamise tegevus
            await _context.LanguageCourses.AddAsync(domain);
            await _context.SaveChangesAsync();
            //tagastatakse domeenile kuuluv objekt
            return domain;
        }
        public async Task<LanguageCourse> Update(LanguageCourseDTO dto)
        {
            if (dto == null || dto.Id == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(dto.Nimetus) || string.IsNullOrEmpty(dto.Keel))
            {
                return null;
            }
            var domain = await _context.LanguageCourses
                .FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (domain == null)
            {
                return null;
            }
            domain.Nimetus = dto.Nimetus;
            domain.Keel = dto.Keel;
            domain.Tase = dto.Tase;
            domain.Kirjeldus = dto.Kirjeldus;
            domain.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return domain;
        }
        public async Task<LanguageCourse> DetailsAsync(Guid id)
        {
            var result = await _context.LanguageCourses
                .FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }
        public async Task<LanguageCourse> Delete(Guid id)
        {
            var domain = await _context.LanguageCourses
                .FirstOrDefaultAsync(x => x.Id == id);
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
