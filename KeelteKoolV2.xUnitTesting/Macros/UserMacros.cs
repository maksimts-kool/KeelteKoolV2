using KeelteKoolV2.Core.Domain;
using Microsoft.AspNetCore.Identity;

namespace KeelteKoolV2.xUnitTesting.Macros
{
    /// <summary>
    /// Makro, mis loob testide ülesseadeks andmebaasi valmis kasutaja.
    /// Kasutus testis: await Svc&lt;UserMacros&gt;().CreateUser();
    /// </summary>
    public class UserMacros : IMacros
    {
        public const string DefaultPassword = "Parool123!";

        private readonly UserManager<ApplicationUser> _userManager;

        public UserMacros(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> CreateUser(
            string email = "test@example.com",
            RegisterStatus status = RegisterStatus.Pending)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                Name = "Test Kasutaja",
                Placeholder = string.Empty,
                AccountStatus = status,
            };

            var result = await _userManager.CreateAsync(user, DefaultPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return user;
        }
    }
}
