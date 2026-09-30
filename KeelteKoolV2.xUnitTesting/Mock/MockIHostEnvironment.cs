using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace KeelteKoolV2.xUnitTesting.Mock
{
    /// <summary>
    /// Testides kasutatav IHostEnvironment, et teenused (nt FileServices)
    /// saaksid küsida keskkonna infot ilma päris veebiserverita.
    /// </summary>
    public class MockIHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Test";
        public string ApplicationName { get; set; } = "KeelteKoolV2.xUnitTesting";
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
