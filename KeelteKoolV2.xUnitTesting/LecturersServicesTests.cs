using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.DTO;
using KeelteKoolV2.Core.ServiceInterface;
using KeelteKoolV2.Data;
using Xunit;

namespace KeelteKoolV2.xUnitTesting;

public class LecturersServicesTests : TestBase
{
    [Fact]
    //create test
    public async Task Should_AddNewLecturer_WhenResultIsReturned()
    {
        //ülesseade
        LecturerDTO newLecturer = MockLecturerDTOData();

        //tegevus
        var result = await Svc<ILecturersServices>().Create(newLecturer);

        //kontroll
        Assert.NotNull(result);
        //teenus genereerib uuele õppejõule id
        Assert.NotEqual(Guid.Empty, result.Id);
        //teenus ei kaota ära andmeid mis me sisestasime
        Assert.Equal(newLecturer.FirstName, result.FirstName);
        Assert.Equal(newLecturer.LastName, result.LastName);
        Assert.Equal(newLecturer.Qualifications, result.Qualifications);
        //loomise ja muutmise aeg on määratud
        Assert.NotEqual(default, result.CreatedAt);
        Assert.NotEqual(default, result.ModifiedAt);
        //õppejõud on andmebaasi salvestatud
        Assert.NotNull(await Svc<ILecturersServices>().DetailsAsync(result.Id));
    }

    [Fact]
    public async Task ShouldNot_AddNewLecturer_WhenFieldsEmpty()
    {
        //ülesseade
        int lecturersBefore = Svc<KeelteKoolV2Context>().Lecturers.Count();
        LecturerDTO newLecturer = MockLecturerDTOData();
        newLecturer.FirstName = string.Empty;
        newLecturer.LastName = string.Empty;
        LecturerDTO noFirstName = MockLecturerDTOData();
        noFirstName.FirstName = string.Empty;
        LecturerDTO noLastName = MockLecturerDTOData();
        noLastName.LastName = string.Empty;

        //tegevus
        var result = await Svc<ILecturersServices>().Create(newLecturer);
        var noFirstNameResult = await Svc<ILecturersServices>().Create(noFirstName);
        var noLastNameResult = await Svc<ILecturersServices>().Create(noLastName);

        //kontroll
        //teenus lükkab ilma nimeta õppejõu lisamise tagasi
        Assert.Null(result);
        //ka ühest puuduvast nimest piisab, et lisamine tagasi lükata
        Assert.Null(noFirstNameResult);
        Assert.Null(noLastNameResult);
        //andmebaasi ei lisatud ühtegi uut õppejõudu/I have no idea if itd work but it should be
        Assert.Equal(lecturersBefore, Svc<KeelteKoolV2Context>().Lecturers.Count());
        Assert.DoesNotContain(Svc<KeelteKoolV2Context>().Lecturers,
            x => x.FirstName == string.Empty || x.LastName == string.Empty);
    }

    [Fact]
    //details test
    public async Task Should_ReturnLecturerDetails_WhenGuidIsNotNull()
    {
        //ülesseade
        LecturerDTO lecturer = MockLecturerDTOData();
        var createdLecturer = await Svc<ILecturersServices>().Create(lecturer);

        //tegevus
        var result = await Svc<ILecturersServices>().DetailsAsync(createdLecturer.Id);

        //kontroll
        Assert.NotNull(result);
        Assert.Equal(result.Id, createdLecturer.Id);
        Assert.Equal(result, createdLecturer);
        //tagastatud õppejõul on samad andmed, mis me sisestasime
        Assert.Equal(lecturer.FirstName, result.FirstName);
        Assert.Equal(lecturer.LastName, result.LastName);
        Assert.Equal(lecturer.Qualifications, result.Qualifications);
    }

    [Fact]
    //update test
    public async Task Should_UpdateWithNewData_WhenDataIsDifferentFromDB()
    {
        Lecturer createdLecturer = await AddObjectToDB();
        //salvestame algse nime enne uuendust, sest InMemory andmebaas tagastab sama jälgitava objekti
        string originalFirstName = createdLecturer.FirstName;
        Svc<KeelteKoolV2Context>().ChangeTracker.Clear();
        LecturerDTO updatedLecturer = new LecturerDTO();
        updatedLecturer.Id = createdLecturer.Id;
        updatedLecturer.FirstName = "Uusnimi";
        updatedLecturer.LastName = createdLecturer.LastName;
        updatedLecturer.Qualifications = createdLecturer.Qualifications;
        updatedLecturer.CreatedAt = createdLecturer.CreatedAt;

        Lecturer result = await Svc<ILecturersServices>().Update(updatedLecturer);

        Assert.NotNull(result);
        Assert.Equal(updatedLecturer.Id, result.Id);
        Assert.Equal(updatedLecturer.LastName, result.LastName);
        Assert.Equal(updatedLecturer.Qualifications, result.Qualifications);
        Assert.DoesNotMatch(originalFirstName, result.FirstName);
    }

    [Fact]
    //delete test
    public async Task Should_DeleteDataFromDB_WhenValidIdGiven()
    {
        Lecturer createdLecturer = await AddObjectToDB();
        Lecturer otherLecturer = await AddObjectToDB();

        var deletedLecturer = await Svc<ILecturersServices>().Delete(createdLecturer.Id);
        var result = await Svc<ILecturersServices>().DetailsAsync(createdLecturer.Id);

        //kustutatud õppejõudu andmebaasist enam ei leita
        Assert.Null(result);
        Assert.Equal(createdLecturer, deletedLecturer);
        Assert.Equal(createdLecturer.Id, deletedLecturer.Id);
        //teine õppejõud jäi andmebaasi alles
        Assert.NotNull(await Svc<ILecturersServices>().DetailsAsync(otherLecturer.Id));
        //juba kustutatud õppejõudu ei saa uuesti kustutada
        Assert.Null(await Svc<ILecturersServices>().Delete(createdLecturer.Id));
    }

    [Fact]
    //oma test: olematu id-ga õppejõu detailid
    public async Task ShouldNot_ReturnLecturerDetails_WhenGuidIsUnknown()
    {
        //ülesseade
        Lecturer createdLecturer = await AddObjectToDB();
        int lecturersBefore = Svc<KeelteKoolV2Context>().Lecturers.Count();

        //tegevus
        var unknownResult = await Svc<ILecturersServices>().DetailsAsync(Guid.NewGuid());
        var emptyResult = await Svc<ILecturersServices>().DetailsAsync(Guid.Empty);
        var existingResult = await Svc<ILecturersServices>().DetailsAsync(createdLecturer.Id);

        //kontroll
        //olematu ja tühja id-ga õppejõudu ei leita
        Assert.Null(unknownResult);
        Assert.Null(emptyResult);
        //olemasolev õppejõud leitakse endiselt üles
        Assert.NotNull(existingResult);
        Assert.Equal(createdLecturer.Id, existingResult.Id);
        //otsimine ei muuda andmebaasi
        Assert.Equal(lecturersBefore, Svc<KeelteKoolV2Context>().Lecturers.Count());
    }

    [Fact]
    //oma test: olematu id-ga õppejõu kustutamine
    public async Task ShouldNot_DeleteLecturer_WhenGuidIsUnknown()
    {
        //ülesseade
        Lecturer createdLecturer = await AddObjectToDB();
        int lecturersBefore = Svc<KeelteKoolV2Context>().Lecturers.Count();

        //tegevus
        var unknownResult = await Svc<ILecturersServices>().Delete(Guid.NewGuid());
        var emptyResult = await Svc<ILecturersServices>().Delete(Guid.Empty);

        //kontroll
        //teenus ei leia midagi kustutamiseks
        Assert.Null(unknownResult);
        Assert.Null(emptyResult);
        //andmebaasist ei kustutatud ühtegi õppejõudu
        Assert.Equal(lecturersBefore, Svc<KeelteKoolV2Context>().Lecturers.Count());
        //olemasolev õppejõud jäi alles samade andmetega
        var existingResult = await Svc<ILecturersServices>().DetailsAsync(createdLecturer.Id);
        Assert.NotNull(existingResult);
        Assert.Equal(createdLecturer.FirstName, existingResult.FirstName);
    }

    [Fact]
    //oma test: tühja (null) andmetega õppejõu lisamine
    public async Task ShouldNot_AddNewLecturer_WhenDataIsNull()
    {
        //ülesseade
        int lecturersBefore = Svc<KeelteKoolV2Context>().Lecturers.Count();
        LecturerDTO noFirstName = MockLecturerDTOData();
        noFirstName.FirstName = null!;
        LecturerDTO noLastName = MockLecturerDTOData();
        noLastName.LastName = null!;

        //tegevus
        var nullResult = await Svc<ILecturersServices>().Create(null!);
        var noFirstNameResult = await Svc<ILecturersServices>().Create(noFirstName);
        var noLastNameResult = await Svc<ILecturersServices>().Create(noLastName);

        //kontroll
        //teenus lükkab tagasi puuduva objekti ja puuduvate nimedega õppejõu
        Assert.Null(nullResult);
        Assert.Null(noFirstNameResult);
        Assert.Null(noLastNameResult);
        //andmebaasi ei lisatud ühtegi uut õppejõudu
        Assert.Equal(lecturersBefore, Svc<KeelteKoolV2Context>().Lecturers.Count());
        Assert.DoesNotContain(Svc<KeelteKoolV2Context>().Lecturers,
            x => x.FirstName == null || x.LastName == null);
    }

    [Fact]
    //oma test: sama õppejõu kaks korda lisamine
    public async Task Should_GiveUniqueIds_WhenSameLecturerIsAddedTwice()
    {
        //ülesseade
        int lecturersBefore = Svc<KeelteKoolV2Context>().Lecturers.Count();
        LecturerDTO lecturer = MockLecturerDTOData();

        //tegevus
        var firstResult = await Svc<ILecturersServices>().Create(lecturer);
        var secondResult = await Svc<ILecturersServices>().Create(lecturer);

        //kontroll
        Assert.NotNull(firstResult);
        Assert.NotNull(secondResult);
        //mõlemad said oma id, kuigi andmed on samad
        Assert.NotEqual(firstResult.Id, secondResult.Id);
        Assert.Equal(firstResult.FirstName, secondResult.FirstName);
        //andmebaasi lisati kaks eraldi õppejõudu
        Assert.Equal(lecturersBefore + 2, Svc<KeelteKoolV2Context>().Lecturers.Count());
        Assert.NotNull(await Svc<ILecturersServices>().DetailsAsync(firstResult.Id));
        Assert.NotNull(await Svc<ILecturersServices>().DetailsAsync(secondResult.Id));
    }

    [Fact]
    //oma test: uuendamine säilitab loomise aja
    public async Task Should_KeepCreatedAt_WhenLecturerIsUpdated()
    {
        //ülesseade
        Lecturer createdLecturer = await AddObjectToDB();
        //salvestame algsed väärtused, sest InMemory andmebaas tagastab sama jälgitava objekti
        DateTime originalCreatedAt = createdLecturer.CreatedAt;
        DateTime originalModifiedAt = createdLecturer.ModifiedAt;
        string? originalQualifications = createdLecturer.Qualifications;
        Svc<KeelteKoolV2Context>().ChangeTracker.Clear();
        LecturerDTO updatedLecturer = new LecturerDTO();
        updatedLecturer.Id = createdLecturer.Id;
        updatedLecturer.FirstName = createdLecturer.FirstName;
        updatedLecturer.LastName = createdLecturer.LastName;
        updatedLecturer.Qualifications = "Inglise keele doktor";
        updatedLecturer.CreatedAt = createdLecturer.CreatedAt;

        //tegevus
        Lecturer result = await Svc<ILecturersServices>().Update(updatedLecturer);
        var fromDB = await Svc<ILecturersServices>().DetailsAsync(createdLecturer.Id);

        //kontroll
        Assert.NotNull(result);
        //loomise aeg jääb samaks, muutmise aeg uueneb
        Assert.Equal(originalCreatedAt, result.CreatedAt);
        Assert.True(result.ModifiedAt >= originalModifiedAt);
        //uus kvalifikatsioon on salvestatud ka andmebaasi
        Assert.NotEqual(originalQualifications, result.Qualifications);
        Assert.NotNull(fromDB);
        Assert.Equal(updatedLecturer.Qualifications, fromDB.Qualifications);
    }

    private async Task<Lecturer> AddObjectToDB()
    {
        return await Svc<ILecturersServices>().Create(MockLecturerDTOData());
    }

    private LecturerDTO MockLecturerDTOData()
    {
        return new LecturerDTO
        {
            FirstName = "Mari",
            LastName = "Tamm",
            Qualifications = "Eesti keele magister"
        };
    }
}
