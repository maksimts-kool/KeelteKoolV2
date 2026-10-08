using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.DTO;

namespace KeelteKoolV2.Core.ServiceInterface;

public interface ILecturersServices
{
    Task<Lecturer> Create(LecturerDTO dto);
    Task<Lecturer> Update(LecturerDTO dto);
    Task<Lecturer> DetailsAsync(Guid id);
    Task<Lecturer> Delete(Guid id);
}
