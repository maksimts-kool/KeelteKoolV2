namespace KeelteKoolV2.Models.Lecturers
{
    public class LecturerViewModel
    {
        public Guid? Id { get; set; } //optional sest index vaade ei vaja seda
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Qualifications { get; set; }
        public string? UserID { get; set; }
        public List<IFormFile>? Files { get; set; }
        public List<LecturerImageViewModel> Image { get; set; } = new List<LecturerImageViewModel>();
        public DateTime? CreatedAt { get; set; } //vajalik uuendamisel, et algne loomise aeg säiliks
    }
}
