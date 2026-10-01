using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KeelteKoolV2.Data
{
    /// <summary>
    /// Development only: applies migrations and creates login-ready accounts
    /// (email already confirmed, so no confirmation mail is needed).
    /// </summary>
    public static class DevelopmentDataSeeder
    {
        private const string AdminRole = "Admin";

        private static readonly (string Email, string Name, string Password, bool IsAdmin)[] Users =
        [
            ("admin@keeltekool.local", "Admin", "Admin123!", true),
            ("user@keeltekool.local", "Test User", "User1234!", false),
        ];

        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<KeelteKoolV2Context>();
            await context.Database.MigrateAsync();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (!await roleManager.RoleExistsAsync(AdminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(AdminRole));
            }

            foreach (var (email, name, password, isAdmin) in Users)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true,
                        Name = name,
                        Placeholder = "seed",
                        AccountStatus = RegisterStatus.Approved
                    };

                    var result = await userManager.CreateAsync(user, password);
                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Seeding user {email} failed: {string.Join("; ", result.Errors.Select(e => e.Description))}");
                    }
                }

                if (isAdmin && !await userManager.IsInRoleAsync(user, AdminRole))
                {
                    await userManager.AddToRoleAsync(user, AdminRole);
                }
            }
        }
    }
}
