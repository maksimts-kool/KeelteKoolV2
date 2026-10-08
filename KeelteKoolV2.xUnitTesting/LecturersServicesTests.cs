using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.DTO;
using KeelteKoolV2.Core.ServiceInterface;
using KeelteKoolV2.Data;
using Xunit;

namespace KeelteKoolV2.xUnitTesting;

public class LecturersServicesTests : TestBase
{
    [Fact]
    public async Task ShouldNot_AddNewLecturer_WhenNamesEmpty()
    {
        LecturerDTO newLecturer = MockLecturerDTOData();
        newLecturer.FirstName = string.Empty;
        newLecturer.LastName = string.Empty;

        var result = await Svc<ILecturersServices>().Create(newLecturer);

        Assert.Null(result);
    }

    [Fact]
    public async Task Should_AddNewLecturer_WhenDataIsValid()
    {
        LecturerDTO newLecturer = MockLecturerDTOData();

        var result = await Svc<ILecturersServices>().Create(newLecturer);

        Assert.NotNull(result);
        Assert.Equal(newLecturer.FirstName, result.FirstName);
        Assert.Equal(newLecturer.LastName, result.LastName);
    }

    [Fact]
    public async Task Should_ReturnLecturerDetails_WhenGuidIsNotNull()
    {
        Lecturer createdLecturer = await AddObjectToDB();

        var result = await Svc<ILecturersServices>().DetailsAsync(createdLecturer.Id);

        Assert.NotNull(result);
        Assert.Equal(createdLecturer.Id, result.Id);
        Assert.Equal(createdLecturer, result);
    }

    [Fact]
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
        Assert.NotEqual(originalFirstName, result.FirstName);
    }

    [Fact]
    public async Task Should_DeleteDataFromDB_WhenValidIdGiven()
    {
        Lecturer createdLecturer = await AddObjectToDB();

        var deletedLecturer = await Svc<ILecturersServices>().Delete(createdLecturer.Id);
        var result = await Svc<ILecturersServices>().DetailsAsync(createdLecturer.Id);

        Assert.Null(result);
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
