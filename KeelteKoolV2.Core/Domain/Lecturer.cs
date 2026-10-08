namespace KeelteKoolV2.Core.Domain;

public class Lecturer
{
    public Guid Id {get; set;}
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public string? Qualifications {get; set;}
    public string? UserID {get; set;}
    //public ICollection<LanguageSubject> LanguageSubjects {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime ModifiedAt {get; set;}
}
