using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.DTO;
using KeelteKoolV2.Core.ServiceInterface;
using KeelteKoolV2.Data;
using Microsoft.EntityFrameworkCore;

namespace KeelteKoolV2.ApplicationServices.Services;

public class LecturersServices : ILecturersServices
{
    private readonly KeelteKoolV2Context _context;
    private readonly IFileServices _fileServices;

    public LecturersServices(KeelteKoolV2Context context, IFileServices fileServices)
    {
        _context = context;
        _fileServices = fileServices;
    }

    public async Task<Lecturer> Create(LecturerDTO dto)
    {
        if (dto == null)
        {
            return null;
        }
        //eesnimi ja perekonnanimi on kohustuslikud
        if (string.IsNullOrEmpty(dto.FirstName) || string.IsNullOrEmpty(dto.LastName))
        {
            return null;
        }

        Lecturer domain = new Lecturer();
        domain.Id = Guid.NewGuid();
        domain.FirstName = dto.FirstName;
        domain.LastName = dto.LastName;
        domain.Qualifications = dto.Qualifications;
        domain.UserID = dto.UserID;
        domain.CreatedAt = DateTime.Now;
        domain.ModifiedAt = DateTime.Now;

        await _context.Lecturers.AddAsync(domain);
        await _context.SaveChangesAsync();
        await _fileServices.UploadFilesToDatabase(dto, domain);
        return domain;
    }

    public async Task<Lecturer> Update(LecturerDTO dto)
    {
        Lecturer domain = new Lecturer();

        domain.Id = (Guid)dto.Id;
        domain.FirstName = dto.FirstName;
        domain.LastName = dto.LastName;
        domain.Qualifications = dto.Qualifications;
        domain.UserID = dto.UserID;
        domain.ModifiedAt = DateTime.Now;
        domain.CreatedAt = (DateTime)dto.CreatedAt;

        _context.Lecturers.Update(domain);
        await _context.SaveChangesAsync();
        await _fileServices.UploadFilesToDatabase(dto, domain);
        return domain;
    }

    public async Task<Lecturer> DetailsAsync(Guid id)
    {
        var result = await _context.Lecturers
            .FirstOrDefaultAsync(x => x.Id == id);
        return result;
    }

    public async Task<Lecturer> Delete(Guid id)
    {
        var domain = await _context.Lecturers
            .FirstOrDefaultAsync(x => x.Id == id);
        if (domain == null)
        {
            return null;
        }
        //kustutame ka õppejõu pildid
        await _fileServices.RemoveImagesByLecturerId(id);
        _context.Lecturers.Remove(domain);
        await _context.SaveChangesAsync();
        return domain;
    }
}
