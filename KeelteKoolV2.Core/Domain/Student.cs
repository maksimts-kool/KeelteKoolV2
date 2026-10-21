namespace KeelteKoolV2.Core.Domain;

public class Student
{
    public Guid Id {get; set;}
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public string Email {get; set;}
    public DateTime DateOfBirth {get; set;}
    public string? UserID {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime ModifiedAt {get; set;}
}
