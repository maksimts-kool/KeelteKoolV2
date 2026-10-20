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
    }

    [Fact]
    //update test
    public async Task Should_UpdateWithNewData_WhenDataIsDifferentFromDB()
    {
        Student createdStudent = await AddObjectToDB();
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
    }

    [Fact]
    //delete test
    public async Task Should_DeleteDataFromDB_WhenValidIdGiven()
    {
        Student createdStudent = await AddObjectToDB();

        await Svc<IStudentsServices>().Delete(createdStudent.Id);
        var result = await Svc<IStudentsServices>().DetailsAsync(createdStudent.Id);

        //kustutatud õpilast andmebaasist enam ei leita
        Assert.Null(result);
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
