using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.DTO;
using KeelteKoolV2.Core.ServiceInterface;
using KeelteKoolV2.Data;
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
            if (domain.Nimetus.Length < 1)
            {
                return null;
            }
            if (domain.Keel.Length < 1)
            {
                return null;
            }
            domain.Kirjeldus = dto.Kirjeldus;
            domain.Tase = dto.Tase;
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
            return null;
        }
        public async Task<LanguageCourse> DetailsAsync(Guid id)
        {
            return null;
        }
        public async Task<LanguageCourse> Delete(Guid id)
        {
            return null;
        }
    }
}
