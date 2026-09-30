using KeelteKoolV2.Core.DTO;
using KeelteKoolV2.Core.ServiceInterface;

namespace KeelteKoolV2.xUnitTesting
{
    public class LanguageCourseUnitTests : TestBase
    {
        // Kursuse lisamisel peab tulemus olema olemas ja andmebaasi salvestatud.
        [Fact]
        public async Task ShouldNot_AddEmptyCourse_WhenResultIsReturned()
        {
            var result = await Svc<ILanguageCoursesServices>().Create(MockCourseData());

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        // Olemasolevat ID-d otsides peab kursuse andmebaasist kätte saama.
        [Fact]
        public async Task Should_GetCourseByID_WhenIdExists()
        {
            var created = await Svc<ILanguageCoursesServices>().Create(MockCourseData());

            var result = await Svc<ILanguageCoursesServices>().DetailAsync(created.Id);

            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
        }

        // Tundmatu ID korral ei tohi kursust tagastada.
        [Fact]
        public async Task ShouldNot_GetCourseByID_WhenIdDoesNotExist()
        {
            var created = await Svc<ILanguageCoursesServices>().Create(MockCourseData());

            var result = await Svc<ILanguageCoursesServices>().DetailAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        // Uuendamisel peavad andmed muutuma, CreatedAt jääb samaks ja ModifiedAt ei tohi väheneda.
        [Fact]
        public async Task Should_UpdateCourse_WhenDataIsChanged()
        {
            var created = await Svc<ILanguageCoursesServices>().Create(MockCourseData());
            var createdAt = created.CreatedAt;
            var modifiedAtBefore = created.ModifiedAt;

            var dto = MockCourseData();
            dto.Id = created.Id;
            dto.Nimetus = "Inglise keel B2";
            dto.Tase = "B2";
            dto.ModifiedBy = "admin";
            var result = await Svc<ILanguageCoursesServices>().Update(dto);

            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
            Assert.Equal("Inglise keel B2", result.Nimetus);
            Assert.Equal("B2", result.Tase);
            Assert.Equal("admin", result.ModifiedBy);
            Assert.Equal(createdAt, result.CreatedAt);
            Assert.True(result.ModifiedAt >= modifiedAtBefore);
        }

        // Olematut kursust ei saa uuendada.
        [Fact]
        public async Task ShouldNot_UpdateCourse_WhenIdDoesNotExist()
        {
            var dto = MockCourseData();
            dto.Id = Guid.NewGuid();

            var result = await Svc<ILanguageCoursesServices>().Update(dto);

            Assert.Null(result);
        }

        // Kustutatud kursust ei tohi andmebaasist enam leida.
        [Fact]
        public async Task Should_RemoveCourseFromDatabase_WhenCourseIsDeleted()
        {
            var created = await Svc<ILanguageCoursesServices>().Create(MockCourseData());

            var deleted = await Svc<ILanguageCoursesServices>().Delete(created.Id);
            var result = await Svc<ILanguageCoursesServices>().DetailAsync(created.Id);

            Assert.NotNull(deleted);
            Assert.Equal(created.Id, deleted.Id);
            Assert.Null(result);
        }

        // Ühe kursuse kustutamine ei tohi teist kursust kustutada.
        [Fact]
        public async Task ShouldNot_RemoveOtherCourse_WhenDifferentCourseIsDeleted()
        {
            var course1 = await Svc<ILanguageCoursesServices>().Create(MockCourseData());
            var course2 = await Svc<ILanguageCoursesServices>().Create(MockCourseData());

            await Svc<ILanguageCoursesServices>().Delete(course2.Id);
            var result = await Svc<ILanguageCoursesServices>().DetailAsync(course1.Id);

            Assert.NotNull(result);
            Assert.Equal(course1.Id, result.Id);
        }

        /* üleval testid, all abimeetodid */

        private LanguageCourseDTO MockCourseData()
        {
            return new LanguageCourseDTO
            {
                Nimetus = "Inglise keel A1",
                Keel = "Inglise",
                Tase = "A1",
                Kirjeldus = "Algajate kursus",
                ModifiedBy = "test",
            };
        }
    }
}
