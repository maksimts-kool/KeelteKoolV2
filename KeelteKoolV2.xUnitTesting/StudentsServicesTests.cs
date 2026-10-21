using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.DTO;
using KeelteKoolV2.Core.ServiceInterface;
using KeelteKoolV2.Data;
using Xunit;

namespace KeelteKoolV2.xUnitTesting;

public class StudentsServicesTests : TestBase
{
    [Fact]
    //create test
    public async Task Should_AddNewStudent_WhenResultIsReturned()
    {
        //ülesseade
        StudentDTO newStudent = MockStudentDTOData();

        //tegevus
        var result = await Svc<IStudentsServices>().Create(newStudent);

        //kontroll
        Assert.NotNull(result);
        //teenus genereerib uuele õpilasele id
        Assert.NotEqual(Guid.Empty, result.Id);
        //teenus ei kaota ära andmeid mis me sisestasime
        Assert.Equal(newStudent.FirstName, result.FirstName);
        Assert.Equal(newStudent.LastName, result.LastName);
        Assert.Equal(newStudent.Email, result.Email);
        Assert.Equal(newStudent.DateOfBirth, result.DateOfBirth);
        //loomise ja muutmise aeg on määratud
        Assert.NotEqual(default, result.CreatedAt);
        Assert.NotEqual(default, result.ModifiedAt);
        //õpilane on andmebaasi salvestatud
        Assert.NotNull(await Svc<IStudentsServices>().DetailsAsync(result.Id));
    }

    [Fact]
    //details test
    public async Task Should_ReturnStudentDetails_WhenGuidIsNotNull()
    {
        //ülesseade
        Student createdStudent = await AddObjectToDB();

        //tegevus
        var result = await Svc<IStudentsServices>().DetailsAsync(createdStudent.Id);

        //kontroll
        Assert.NotNull(result);
        Assert.Equal(createdStudent.Id, result.Id);
        Assert.Equal(createdStudent.FirstName, result.FirstName);
        Assert.Equal(createdStudent.LastName, result.LastName);
        Assert.Equal(createdStudent.Email, result.Email);
        Assert.Equal(createdStudent.DateOfBirth, result.DateOfBirth);
        Assert.Equal(createdStudent.CreatedAt, result.CreatedAt);
    }

    [Fact]
    //update test
    public async Task Should_UpdateWithNewData_WhenDataIsDifferentFromDB()
    {
        Student createdStudent = await AddObjectToDB();
        //salvestame algse nime enne uuendust, sest InMemory andmebaas tagastab sama jälgitava objekti
        string originalFirstName = createdStudent.FirstName;
        //kõik testid jagavad ühte andmebaasi konteksti, seega eemaldame jälgimisest, et Update saaks sama id-ga objekti uuesti lisada
        Svc<KeelteKoolV2Context>().ChangeTracker.Clear();
        StudentDTO updatedStudent = new StudentDTO();
        updatedStudent.Id = createdStudent.Id;
        updatedStudent.FirstName = "Uusnimi";
        updatedStudent.LastName = createdStudent.LastName;
        updatedStudent.Email = createdStudent.Email;
        updatedStudent.DateOfBirth = createdStudent.DateOfBirth;
        updatedStudent.CreatedAt = createdStudent.CreatedAt;

        Student result = await Svc<IStudentsServices>().Update(updatedStudent);

        Assert.NotNull(result);
        Assert.Equal(updatedStudent.Id, result.Id);
        Assert.Equal("Uusnimi", result.FirstName);
        Assert.NotEqual(originalFirstName, result.FirstName);
        Assert.Equal(updatedStudent.LastName, result.LastName);
        Assert.Equal(updatedStudent.Email, result.Email);
        Assert.Equal(updatedStudent.DateOfBirth, result.DateOfBirth);
        Assert.Equal(updatedStudent.CreatedAt, result.CreatedAt);
    }

    [Fact]
    //delete test
    public async Task Should_DeleteDataFromDB_WhenValidIdGiven()
    {
        Student createdStudent = await AddObjectToDB();

        var deletedStudent = await Svc<IStudentsServices>().Delete(createdStudent.Id);
        var result = await Svc<IStudentsServices>().DetailsAsync(createdStudent.Id);

        //kustutatud õpilast andmebaasist enam ei leita
        Assert.Null(result);
        //teenus tagastab kustutatud õpilase
        Assert.NotNull(deletedStudent);
        Assert.Equal(createdStudent.Id, deletedStudent.Id);
        Assert.Equal(createdStudent.FirstName, deletedStudent.FirstName);
        Assert.Equal(createdStudent.LastName, deletedStudent.LastName);
        Assert.Equal(createdStudent.Email, deletedStudent.Email);
    }

    [Fact]
    //oma test: kohustuslike väljadeta õpilast ei lisata
    public async Task ShouldNot_AddNewStudent_WhenRequiredFieldIsEmpty()
    {
        //ülesseade
        StudentDTO noFirstName = MockStudentDTOData();
        noFirstName.FirstName = string.Empty;
        StudentDTO noLastName = MockStudentDTOData();
        noLastName.LastName = string.Empty;
        StudentDTO noEmail = MockStudentDTOData();
        noEmail.Email = string.Empty;

        //tegevus
        var resultNoFirstName = await Svc<IStudentsServices>().Create(noFirstName);
        var resultNoLastName = await Svc<IStudentsServices>().Create(noLastName);
        var resultNoEmail = await Svc<IStudentsServices>().Create(noEmail);
        var resultNoDto = await Svc<IStudentsServices>().Create(null);
        var resultValid = await Svc<IStudentsServices>().Create(MockStudentDTOData());

        //kontroll
        Assert.Null(resultNoFirstName);
        Assert.Null(resultNoLastName);
        Assert.Null(resultNoEmail);
        Assert.Null(resultNoDto);
        //kõigi väljadega õpilane lisatakse ikkagi
        Assert.NotNull(resultValid);
    }

    [Fact]
    //oma test: tundmatu id-ga õpilast ei leita
    public async Task Should_ReturnNull_WhenStudentIdDoesNotExist()
    {
        //ülesseade
        Student createdStudent = await AddObjectToDB();
        Guid unknownId = Guid.NewGuid();

        //tegevus
        var resultUnknown = await Svc<IStudentsServices>().DetailsAsync(unknownId);
        var resultEmpty = await Svc<IStudentsServices>().DetailsAsync(Guid.Empty);
        var resultExisting = await Svc<IStudentsServices>().DetailsAsync(createdStudent.Id);

        //kontroll
        Assert.Null(resultUnknown);
        Assert.Null(resultEmpty);
        Assert.NotEqual(createdStudent.Id, unknownId);
        //olemasolev õpilane leitakse ikkagi
        Assert.NotNull(resultExisting);
        Assert.Equal(createdStudent.Id, resultExisting.Id);
    }

    [Fact]
    //oma test: tundmatu id kustutamine ei kustuta midagi
    public async Task ShouldNot_DeleteAnything_WhenStudentIdDoesNotExist()
    {
        //ülesseade
        Student createdStudent = await AddObjectToDB();

        //tegevus
        var resultUnknown = await Svc<IStudentsServices>().Delete(Guid.NewGuid());
        var resultEmpty = await Svc<IStudentsServices>().Delete(Guid.Empty);
        var stillExisting = await Svc<IStudentsServices>().DetailsAsync(createdStudent.Id);

        //kontroll
        Assert.Null(resultUnknown);
        Assert.Null(resultEmpty);
        //olemasolev õpilane jääb andmebaasi alles muutmata kujul
        Assert.NotNull(stillExisting);
        Assert.Equal(createdStudent.Id, stillExisting.Id);
        Assert.Equal(createdStudent.Email, stillExisting.Email);
    }

    [Fact]
    //oma test: uuendamine säilitab loomise aja ja muudab muutmise aja
    public async Task Should_KeepCreatedAtAndChangeModifiedAt_WhenStudentIsUpdated()
    {
        //ülesseade
        Student createdStudent = await AddObjectToDB();
        DateTime originalCreatedAt = createdStudent.CreatedAt;
        DateTime originalModifiedAt = createdStudent.ModifiedAt;
        Svc<KeelteKoolV2Context>().ChangeTracker.Clear();
        //ootame natuke, et uus muutmise aeg oleks kindlasti hilisem
        await Task.Delay(10);
        StudentDTO updatedStudent = MockStudentDTOData();
        updatedStudent.Id = createdStudent.Id;
        updatedStudent.Email = "uus.email@example.com";
        updatedStudent.CreatedAt = createdStudent.CreatedAt;

        //tegevus
        Student result = await Svc<IStudentsServices>().Update(updatedStudent);
        Svc<KeelteKoolV2Context>().ChangeTracker.Clear();
        Student fromDB = await Svc<IStudentsServices>().DetailsAsync(createdStudent.Id);

        //kontroll
        Assert.NotNull(result);
        Assert.Equal(originalCreatedAt, result.CreatedAt);
        Assert.True(result.ModifiedAt > originalModifiedAt);
        //muudatus on andmebaasi salvestatud
        Assert.NotNull(fromDB);
        Assert.Equal("uus.email@example.com", fromDB.Email);
        Assert.Equal(originalCreatedAt, fromDB.CreatedAt);
    }

    [Fact]
    //oma test: kaks samade andmetega õpilast saavad erineva id
    public async Task Should_GiveDifferentIds_WhenTwoStudentsAreCreated()
    {
        //ülesseade
        StudentDTO firstStudent = MockStudentDTOData();
        StudentDTO secondStudent = MockStudentDTOData();

        //tegevus
        var firstResult = await Svc<IStudentsServices>().Create(firstStudent);
        var secondResult = await Svc<IStudentsServices>().Create(secondStudent);

        //kontroll
        Assert.NotNull(firstResult);
        Assert.NotNull(secondResult);
        Assert.NotEqual(firstResult.Id, secondResult.Id);
        Assert.Equal(firstResult.Email, secondResult.Email);
        //mõlemad õpilased on andmebaasis eraldi olemas
        Assert.NotNull(await Svc<IStudentsServices>().DetailsAsync(firstResult.Id));
        Assert.NotNull(await Svc<IStudentsServices>().DetailsAsync(secondResult.Id));
    }

    private async Task<Student> AddObjectToDB()
    {
        //lisab andmebaasi testõpilase teenuse kaudu ja tagastab selle
        return await Svc<IStudentsServices>().Create(MockStudentDTOData());
    }

    private StudentDTO MockStudentDTOData()
    {
        return new StudentDTO
        {
            FirstName = "Juku",
            LastName = "Kask",
            Email = "juku.kask@example.com",
            DateOfBirth = new DateTime(2005, 5, 15)
        };
    }
}
