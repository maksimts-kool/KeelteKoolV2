using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.DTO;

namespace KeelteKoolV2.Core.ServiceInterface;

public interface IStudentsServices
{
    Task<Student> Create(StudentDTO dto);
    Task<Student> Update(StudentDTO dto);
    Task<Student> DetailsAsync(Guid id);
    Task<Student> Delete(Guid id);
}
