using System.ComponentModel.DataAnnotations;

namespace KeelteKoolV2.Core.Domain;

public class FileToDatabase
{
    [Key]
    public Guid ImageID {get; set;}
    public string? ImageTitle {get; set;}
    public byte[]? ImageData {get; set;}
    public Guid? LecturerId {get; set;}
}
