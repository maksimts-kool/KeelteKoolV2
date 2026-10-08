using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.DTO;

namespace KeelteKoolV2.Core.ServiceInterface;

public interface IFileServices
{
    Task UploadFilesToDatabase(LecturerDTO dto, Lecturer domain);
    Task<FileToDatabase> RemoveImage(FileToDatabaseDTO dto);
    Task RemoveImagesByLecturerId(Guid lecturerId);
}
