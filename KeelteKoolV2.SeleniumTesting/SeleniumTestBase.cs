using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;

namespace KeelteKoolV2.SeleniumTesting
{
    // Ühine alus kõigile UI testidele (keelekursused, õppejõud, õpilased):
    // rakenduse aadress, brauseri draiver, ootamised ja koristamine pärast testi.
    // Iga test saab oma brauseri, sest xUnit loob iga testi jaoks uue klassi eksemplari.
    public abstract class SeleniumTestBase : IDisposable
    {
        // SELENIUM_BASE_URL=<aadress> testib mõnda teist käivitatud rakendust
        protected static readonly string BaseUrl = WithTrailingSlash(
            Environment.GetEnvironmentVariable("SELENIUM_BASE_URL") ?? "http://localhost:5117/");

        // SELENIUM_WAIT_SECONDS=<sekundit> muudab, kaua elementi oodatakse enne testi kukkumist
        protected static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(
            int.TryParse(Environment.GetEnvironmentVariable("SELENIUM_WAIT_SECONDS"), out int seconds) && seconds > 0
                ? seconds
                : 10);

        protected readonly IWebDriver driver;
        protected readonly WebDriverWait wait;

        protected SeleniumTestBase()
        {
            var options = new FirefoxOptions();
            // SELENIUM_HEADLESS=true peidab brauseri akna (nt CI jaoks), vaikimisi on brauser nähtav
            if (string.Equals(Environment.GetEnvironmentVariable("SELENIUM_HEADLESS"),
                    "true", StringComparison.OrdinalIgnoreCase))
            {
                options.AddArgument("-headless");
            }

            var firefoxDriver = new FirefoxDriver(options);

            // SELENIUM_SLOWMO=<millisekundit> aeglustab iga klikki, tekstisisestust ja navigeerimist, et brauserit jälgida
            if (int.TryParse(Environment.GetEnvironmentVariable("SELENIUM_SLOWMO"), out int slowMoMs) && slowMoMs > 0)
            {
                var eventDriver = new EventFiringWebDriver(firefoxDriver);
                eventDriver.ElementClicked += (_, _) => Thread.Sleep(slowMoMs);
                eventDriver.ElementValueChanged += (_, _) => Thread.Sleep(slowMoMs);
                eventDriver.Navigated += (_, _) => Thread.Sleep(slowMoMs);
                driver = eventDriver;
            }
            else
            {
                driver = firefoxDriver;
            }

            driver.Manage().Timeouts().ImplicitWait = WaitTimeout;
            wait = new WebDriverWait(driver, WaitTimeout);
        }

        // avab rakenduse lehe, nt GoTo("Students") või GoTo("") avalehe jaoks
        protected void GoTo(string relativePath)
        {
            driver.Url = BaseUrl + relativePath.TrimStart('/');
        }

        // ootab, kuni element on lehel nähtav, ja tagastab selle
        protected IWebElement WaitUntilVisible(By by)
        {
            return wait.Until(d =>
            {
                IWebElement element = d.FindElement(by);
                return element.Displayed ? element : null;
            })!;
        }

        // ootab, kuni brauseri aadress sisaldab antud teksti, nt pärast vormi salvestamist
        protected void WaitUntilUrlContains(string fragment)
        {
            wait.Until(d => d.Url.Contains(fragment, StringComparison.OrdinalIgnoreCase));
        }

        // unikaalne väärtus, et test leiaks just enda loodud rea, mitte mõne vana rea andmebaasist
        protected static string UniqueValue(string prefix)
        {
            return $"{prefix}{Guid.NewGuid():N}";
        }

        // alamklass kustutab siin oma testiandmed; jookseb ka siis, kui test kukkus
        protected virtual void CleanUp()
        {
        }

        public void Dispose()
        {
            try
            {
                CleanUp();
            }
            finally
            {
                driver.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        private static string WithTrailingSlash(string url)
        {
            return url.EndsWith('/') ? url : url + "/";
        }
    }
}
