using KeelteKoolV2.ApplicationServices.Services;
using KeelteKoolV2.Core.Domain;
using KeelteKoolV2.Core.ServiceInterface;
using KeelteKoolV2.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using KeelteKoolV2.xUnitTesting.Macros;
using KeelteKoolV2.xUnitTesting.Mock;

namespace KeelteKoolV2.xUnitTesting
{
    public abstract class TestBase
    {
        protected IServiceProvider serviceProvider { get; set; }

        protected TestBase()
        {
            var services = new ServiceCollection();
            SetupServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Seame üles testideks vajalikud teenused (Program.cs lühendatud kujul).
        /// Andmebaas on mälus ja iga testiklassi instants saab oma unikaalse andmebaasi,
        /// et testid üksteist ei segaks.
        /// </summary>
        /// <param name="services">tühi ServiceCollection, kuhu teenused lisatakse</param>
        public virtual void SetupServices(ServiceCollection services)
        {
            var databaseName = Guid.NewGuid().ToString();

            services.AddLogging();

            services.AddScoped<ILanguageCoursesServices, LanguageCoursesServices>();
            //services.AddScoped<IFileServices, FileServices>();
            services.AddScoped<IHostEnvironment, MockIHostEnvironment>();

            services.AddDbContext<KeelteKoolV2Context>(x =>
            {
                x.UseInMemoryDatabase(databaseName);
                x.ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            services.AddIdentityCore<ApplicationUser>()
                .AddEntityFrameworkStores<KeelteKoolV2Context>();

            RegisterMacros(services);
        }

        /// <summary>
        /// Leia teenusepakkujalt X tüüpi teenus.
        /// </summary>
        protected T Svc<T>() where T : notnull
        {
            return serviceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// Registreerib kõik IMacros liidest realiseerivad klassid teenustena
        /// (v.a liidesed ja abstraktsed klassid). Makro --> Teenus
        /// </summary>
        /// <param name="services">Teenused, kuhu makrod lisatakse</param>
        private void RegisterMacros(ServiceCollection services)
        {
            var macroBaseType = typeof(IMacros);

            var macros = macroBaseType.Assembly.GetTypes()
                .Where(t => macroBaseType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var macro in macros)
            {
                services.AddScoped(macro);
            }
        }
    }
}
