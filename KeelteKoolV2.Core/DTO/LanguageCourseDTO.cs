namespace KeelteKoolV2.Core.DTO
{
    public class LanguageCourseDTO
    {
        //Andmevahendusobjekt, ei pea vastama andmebaasis nõutud andmetele,
        //selle eesmärk on frontendi kontrolleri ja backendi teenuse vahel
        //andmete üle andmine, osad andmed võivad olla valikulised (märgitud "?" märgiga),
        //kuna service või kontroller saab omalt poolt midagi vajadusel muuta
        // või juurde lisada millel lõppkasutajal juurdepääsu olla ei tohiks.
        public Guid? Id { get; set; }
        public string Nimetus { get; set; } = string.Empty;
        public string Keel { get; set; } = string.Empty;
        public string Tase { get; set; } = string.Empty;
        public string Kirjeldus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
        public string ModifiedBy { get; set; } = string.Empty;
    }
}
