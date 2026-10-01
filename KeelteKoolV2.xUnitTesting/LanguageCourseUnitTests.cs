using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.DTO;
using KeelteKoolV2.Core.ServiceInterface;
using KeelteKoolV2.Data;
using Microsoft.Extensions.DependencyInjection;

namespace KeelteKoolV2.xUnitTesting
{
    public class LanguageCourseUnitTests : TestBase
    {
        // Selles testis kontrollitakse et (2) kursuse lisamisel
        // (1) peaks (3) tagastatud tulemus sisaldama kõiki DTO-s antud andmeid.
        [Fact]
        public async Task Should_AddNewCourse_WhenResultIsReturned()
        {
            //ülesseade
            LanguageCourseDTO newCourseDTO = MockLanguageCourseDTO();

            //tegevus
            var result = await Svc<ILanguageCoursesServices>().Create(newCourseDTO);

            //kontroll
            Assert.NotNull(result);
            /*
            Assert on klass mille abil saab kontrollita andmete eri tingimusi, kujusid, olekuid jne.
            Antud juhul kontrollitakse eelnevat objekti ühe kontrolliga - et ei oleks tühi.
            Aga, kui meie meetod pärast selle sisu arendamist hakkab juba tagastama mingisugust objekti,
            tuleks testi täiendada, täpsemate tingimustega, mis kontrollib näiteks, kas on samasugune,
            sisaldab kindlal kujul andmeid, andmed on mingit kindlat tüüpi jne. Võimalusi mida kontrollida on palju,
            ning viise kuidas teste kirjutada veelgi rohkem.
            */
        }

        [Fact]
        public async Task ShouldNot_AddNewCourse_WhenFieldsAreEmpty()
        {
            //ülesseade
            LanguageCourseDTO newCourseDTO = MockLanguageCourseDTO();
            newCourseDTO.Nimetus = string.Empty;
            newCourseDTO.Keel = string.Empty;

            //tegevus
            var result = await Svc<ILanguageCoursesServices>().Create(newCourseDTO);

            //kontroll
            Assert.Null(result);
            if (result != null)
            {
            Assert.NotNull(result.Keel);
            Assert.NotNull(result.Nimetus);
            Assert.True(result.Keel.Length > 0);
            Assert.True(result.Nimetus.Length < 1);
            Assert.Equal(newCourseDTO.Keel, result.Keel);
            }
        }
        private LanguageCourseDTO MockLanguageCourseDTO()
        {
            return new LanguageCourseDTO
            {
                Nimetus = "Inglise keel B2",
                Keel = "Inglise",
                Tase = "B2",
                Kirjeldus = "Testkirjeldus"
            };
        }
        [Fact]
        public async Task Should_ReturnCourseDetails_WhenGuidIsNotNull()
        {}
    }
}