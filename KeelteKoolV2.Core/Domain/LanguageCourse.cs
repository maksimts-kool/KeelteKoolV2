namespace KeelteKoolV2.Core.Domain
{
    public class LanguageCourse
    {
        //Domain kaustas olev mudel näitab ära täpse andmekuju millena kursuse andmed andmebaasis ka
        //hoitakse, siin võib samuti olla valikulisi andmeid, kuid tüüpiliselt on neid vähem
        //sest andmebaas hoiab tihtipeale ainult vajalikke andmeid.
        public Guid Id { get; set; }
        public string Nimetus { get; set; } = string.Empty;
        public string Keel { get; set; } = string.Empty;
        public string Tase { get; set; } = string.Empty;
        public string Kirjeldus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
        public string ModifiedBy { get; set; } = string.Empty;
    }
}
