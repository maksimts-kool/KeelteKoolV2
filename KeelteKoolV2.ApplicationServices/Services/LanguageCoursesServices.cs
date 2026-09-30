//using KeelteKoolV2.Core.Domain;
//using KeelteKoolV2.Core.DTO;
//using KeelteKoolV2.Core.ServiceInterface;
//using KeelteKoolV2.Data;
//using Microsoft.EntityFrameworkCore;

//namespace KeelteKoolV2.ApplicationServices.Services
//{
//    // Pole veel rakendatud - eeldab LanguageCourse DbSet-i KeelteKoolV2Context-is
//    public class LanguageCoursesServices : ILanguageCoursesServices
//    {
//        private readonly KeelteKoolV2Context _context;

//        public LanguageCoursesServices(KeelteKoolV2Context context)
//        {
//            _context = context;
//        }

//        public async Task<LanguageCourse> Create(LanguageCourseDTO dto)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<LanguageCourse?> DetailAsync(Guid id)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<LanguageCourse> Update(LanguageCourseDTO dto)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<LanguageCourse?> Delete(Guid id)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
