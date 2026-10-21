using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.DTO;
using KeelteKoolV2.Core.ServiceInterface;
using KeelteKoolV2.Data;
using Microsoft.EntityFrameworkCore;

namespace KeelteKoolV2.ApplicationServices.Services;

public class StudentsServices : IStudentsServices
{
    private readonly KeelteKoolV2Context _context;

    public StudentsServices(KeelteKoolV2Context context)
    {
        _context = context;
    }

    public async Task<Student> Create(StudentDTO dto)
    {
        if (dto == null)
        {
            return null;
        }
        //eesnimi, perekonnanimi ja e-post on kohustuslikud
        if (string.IsNullOrEmpty(dto.FirstName) || string.IsNullOrEmpty(dto.LastName) || string.IsNullOrEmpty(dto.Email))
        {
            return null;
        }

        Student domain = new Student();
        domain.Id = Guid.NewGuid();
        domain.FirstName = dto.FirstName;
        domain.LastName = dto.LastName;
        domain.Email = dto.Email;
        domain.DateOfBirth = dto.DateOfBirth;
        domain.UserID = dto.UserID;
        domain.CreatedAt = DateTime.Now;
        domain.ModifiedAt = DateTime.Now;

        await _context.Students.AddAsync(domain);
        await _context.SaveChangesAsync();
        return domain;
    }

    public async Task<Student> Update(StudentDTO dto)
    {
        Student domain = new Student();

        domain.Id = (Guid)dto.Id;
        domain.FirstName = dto.FirstName;
        domain.LastName = dto.LastName;
        domain.Email = dto.Email;
        domain.DateOfBirth = dto.DateOfBirth;
        domain.UserID = dto.UserID;
        domain.ModifiedAt = DateTime.Now;
        domain.CreatedAt = (DateTime)dto.CreatedAt;

        _context.Students.Update(domain);
        await _context.SaveChangesAsync();
        return domain;
    }

    public async Task<Student> DetailsAsync(Guid id)
    {
        var result = await _context.Students
            .FirstOrDefaultAsync(x => x.Id == id);
        return result;
    }

    public async Task<Student> Delete(Guid id)
    {
        var domain = await _context.Students
            .FirstOrDefaultAsync(x => x.Id == id);
        if (domain == null)
        {
            return null;
        }
        _context.Students.Remove(domain);
        await _context.SaveChangesAsync();
        return domain;
    }
}
