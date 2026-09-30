using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.DTO;

namespace KeelteKoolV2.Core.ServiceInterface
{
    public interface ILanguageCoursesServices
    {
        Task<List<LanguageCourse>> GetAllAsync();
        Task<LanguageCourse> Create(LanguageCourseDTO dto);
        Task<LanguageCourse?> DetailAsync(Guid id);
        Task<LanguageCourse?> Update(LanguageCourseDTO dto);
        Task<LanguageCourse?> Delete(Guid id);
    }
}
