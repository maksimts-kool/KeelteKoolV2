namespace KeelteKoolV2.Models.Lecturers
{
    public class LecturerImageViewModel
    {
        public Guid ImageID { get; set; }
        public string? ImageTitle { get; set; }
        public byte[]? ImageData { get; set; }
        public string? Image { get; set; } //base64 kujul, kuvamiseks <img> elemendis
        public Guid? LecturerId { get; set; }
    }
}
