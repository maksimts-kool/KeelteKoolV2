using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.DTO;
using KeelteKoolV2.Core.ServiceInterface;
using KeelteKoolV2.Data;
using Xunit;

namespace KeelteKoolV2.xUnitTesting;

public class LecturersServicesTests : TestBase
{
    [Fact]
    public async Task ShouldNot_AddNewLecturer_WhenResultIsReturned()
    {
        LecturerDTO newLecturer = MockLecturerDTOData();

        var result = await Svc<ILecturersServices>().Create(newLecturer);

        Assert.NotNull(result);
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

        var deletedLecturer = await Svc<ILecturersServices>().Delete(createdLecturer.Id);
        var result = await Svc<ILecturersServices>().DetailsAsync(createdLecturer.Id);

        Assert.Null(result);
        Assert.Equal(createdLecturer, deletedLecturer);
        Assert.Equal(createdLecturer.Id, deletedLecturer.Id);
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
