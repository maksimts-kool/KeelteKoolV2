namespace KeelteKoolV2.Core.DTO;

public class StudentDTO
{
    public Guid? Id {get; set;}
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public string Email {get; set;}
    public DateTime DateOfBirth {get; set;}
    public string? UserID {get; set;}
    public DateTime? CreatedAt {get; set;}
    public DateTime? ModifiedAt {get; set;}
}
