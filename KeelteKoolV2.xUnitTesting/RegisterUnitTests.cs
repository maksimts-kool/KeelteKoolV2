using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Data;
using KeelteKoolV2.xUnitTesting.Macros;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KeelteKoolV2.xUnitTesting
{
    public class RegisterUnitTests : TestBase
    {
        private const string ValidPassword = "Parool123!";

        // Selles testis kontrollitakse et (2) kasutaja registreerimisel
        // (1) ei tohiks (3) tulemus olla ebaõnnestunud, kui andmed on korrektsed.
        [Fact]
        public async Task ShouldNot_FailRegistration_WhenDataIsValid()
        {
            // ülesseade
            var user = MockUser();

            // tegevus
            var result = await Svc<UserManager<ApplicationUser>>().CreateAsync(user, ValidPassword);

            // kontroll
            Assert.True(result.Succeeded);
        }

        // Uus kasutaja peab jääma staatusesse Pending, kuni admin kinnitab.
        [Fact]
        public async Task Should_HavePendingStatus_WhenUserIsRegistered()
        {
            var user = MockUser();
            await Svc<UserManager<ApplicationUser>>().CreateAsync(user, ValidPassword);

            var saved = await Svc<UserManager<ApplicationUser>>().FindByEmailAsync(user.Email!);

            Assert.NotNull(saved);
            Assert.Equal(RegisterStatus.Pending, saved.AccountStatus);
        }

        // Sama e-posti aadressiga ei tohi kahte kasutajat registreerida.
        [Fact]
        public async Task ShouldNot_RegisterUser_WhenEmailIsAlreadyTaken()
        {
            var userManager = Svc<UserManager<ApplicationUser>>();
            await userManager.CreateAsync(MockUser(), ValidPassword);

            var result = await userManager.CreateAsync(MockUser(), ValidPassword);

            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Code == nameof(IdentityErrorDescriber.DuplicateUserName));
        }

        // Liiga nõrga parooliga registreerimine peab ebaõnnestuma.
        [Fact]
        public async Task ShouldNot_RegisterUser_WhenPasswordIsTooWeak()
        {
            var result = await Svc<UserManager<ApplicationUser>>().CreateAsync(MockUser(), "1");

            Assert.False(result.Succeeded);
        }

        // Konto staatuse muutmine (nt Pending -> Approved) peab andmebaasi jõudma.
        [Fact]
        public async Task Should_PersistApprovedStatus_WhenAccountStatusIsUpdated()
        {
            var userManager = Svc<UserManager<ApplicationUser>>();
            var user = MockUser();
            await userManager.CreateAsync(user, ValidPassword);

            user.AccountStatus = RegisterStatus.Approved;
            var updateResult = await userManager.UpdateAsync(user);

            var fromDb = await Svc<KeelteKoolV2Context>().Users.SingleAsync(u => u.Id == user.Id);
            Assert.True(updateResult.Succeeded);
            Assert.Equal(RegisterStatus.Approved, fromDb.AccountStatus);
        }

        // Kustutatud kasutajat ei tohi andmebaasist enam leida.
        [Fact]
        public async Task Should_RemoveUserFromDatabase_WhenUserIsDeleted()
        {
            var userManager = Svc<UserManager<ApplicationUser>>();
            var user = MockUser();
            await userManager.CreateAsync(user, ValidPassword);

            await userManager.DeleteAsync(user);
            var result = await userManager.FindByIdAsync(user.Id);

            Assert.Null(result);
        }

        // Makro peab looma andmebaasi kasutaja, mille staatus vastab soovitule.
        [Fact]
        public async Task Should_CreateUserInDatabase_WhenUserMacroIsUsed()
        {
            var user = await Svc<UserMacros>().CreateUser(status: RegisterStatus.Approved);

            var fromDb = await Svc<UserManager<ApplicationUser>>().FindByIdAsync(user.Id);

            Assert.NotNull(fromDb);
            Assert.Equal(RegisterStatus.Approved, fromDb.AccountStatus);
        }

        /* üleval testid, all abimeetodid */

        private ApplicationUser MockUser()
        {
            return new ApplicationUser
            {
                UserName = "test@example.com",
                Email = "test@example.com",
                Name = "Test Kasutaja",
                Placeholder = string.Empty,
            };
        }
    }
}
