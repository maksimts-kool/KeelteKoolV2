using System.ComponentModel.DataAnnotations;

namespace KeelteKoolV2.Models.LanguageCourses
{
    //ViewModel on vajalik kasutajale info kuvamiseks, ja sealt edasi controllerile andmiseks, Viewmodel
    //erineb DTO-objektist selle võrra, et kõik kasutajale mittevajalikud andmed on sealt eemaldatud.
    //Valikulised andmed mis kuuluvad ka ka hiljem kasutajatele esitamiseks siiski jäävad.
    //See eraldatus tagab ka selle et kasutaja ei saa pahatahtlikult soovimatutele andmetele ligipääsu.
    public class LanguageCourseViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Nimetus on kohustuslik.")]
        [Display(Name = "Nimetus")]
        public string Nimetus { get; set; } = string.Empty;

        [Required(ErrorMessage = "Keel on kohustuslik.")]
        [Display(Name = "Keel")]
        public string Keel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tase on kohustuslik.")]
        [Display(Name = "Tase")]
        public string Tase { get; set; } = string.Empty;

        [Display(Name = "Kirjeldus")]
        public string Kirjeldus { get; set; } = string.Empty;
    }
}
