using KeelteKoolV2.ApplicationServices.Services;
using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.ServiceInterface;
using KeelteKoolV2.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KeelteKoolV2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //services
            builder.Services.AddScoped<IEmailingServices, EmailingServices>();
            builder.Services.AddScoped<ILanguageCoursesServices, LanguageCoursesServices>();
            builder.Services.AddScoped<ILecturersServices, LecturersServices>();
            builder.Services.AddScoped<IFileServices, FileServices>();
            //apiclients

            //dbcontext
            builder.Services.AddDbContext<KeelteKoolV2Context>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            //identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
                options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Password.RequiredLength = 8;
                    options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
                    options.Lockout.MaxFailedAccessAttempts = 3;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                })
                .AddEntityFrameworkStores<KeelteKoolV2Context>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("CustomEmailConfirmation");

            //auth


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
