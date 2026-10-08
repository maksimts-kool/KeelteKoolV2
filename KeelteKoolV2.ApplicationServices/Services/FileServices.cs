using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.DTO;
using KeelteKoolV2.Core.ServiceInterface;
using KeelteKoolV2.Data;
using Microsoft.EntityFrameworkCore;

namespace KeelteKoolV2.ApplicationServices.Services;

public class FileServices : IFileServices
{
    private readonly KeelteKoolV2Context _context;

    public FileServices(KeelteKoolV2Context context)
    {
        _context = context;
    }

    public async Task UploadFilesToDatabase(LecturerDTO dto, Lecturer domain)
    {
        //kui faile ei ole, pole midagi salvestada
        if (dto.Files == null || dto.Files.Count == 0)
        {
            return;
        }
        foreach (var file in dto.Files)
        {
            using (var target = new MemoryStream())
            {
                FileToDatabase files = new FileToDatabase
                {
                    ImageID = Guid.NewGuid(),
                    ImageTitle = file.FileName,
                    LecturerId = domain.Id
                };
                await file.CopyToAsync(target);
                files.ImageData = target.ToArray();
                await _context.Files.AddAsync(files);
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task<FileToDatabase> RemoveImage(FileToDatabaseDTO dto)
    {
        var image = await _context.Files
            .FirstOrDefaultAsync(x => x.ImageID == dto.ImageID);
        if (image == null)
        {
            return null;
        }
        _context.Files.Remove(image);
        await _context.SaveChangesAsync();
        return image;
    }

    public async Task RemoveImagesByLecturerId(Guid lecturerId)
    {
        var images = await _context.Files
            .Where(x => x.LecturerId == lecturerId)
            .ToListAsync();
        _context.Files.RemoveRange(images);
        await _context.SaveChangesAsync();
    }
}
