using System.ComponentModel.DataAnnotations;

namespace KeelteKoolV2.Models.Students
{
    public class StudentViewModel
    {
        public Guid? Id { get; set; } //optional sest index vaade ei vaja seda
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [Required]
        public DateTime? DateOfBirth { get; set; } //nullable, et uue õpilase vormis poleks vaikimisi kuupäeva 01.01.0001
        public string? UserID { get; set; }
        public DateTime? CreatedAt { get; set; } //vajalik uuendamisel, et algne loomise aeg säiliks
    }
}
