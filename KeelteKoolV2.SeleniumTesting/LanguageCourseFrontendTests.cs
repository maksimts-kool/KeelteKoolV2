using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.Events;
using Xunit;

namespace KeelteKoolV2.SeleniumTesting
{
    public class LanguageCourseFrontendTests : IDisposable
    {
        private static readonly string BaseUrl = Environment.GetEnvironmentVariable("SELENIUM_BASE_URL")
            ?? "http://localhost:5117/"; //navigeeritav aadress
        // indeks näitab 20 kursust Keele järgi sorteeritult, numbriga algav keel hoiab testkursused nähtaval
        private const string CourseKeel = "0 Selenium";

        private readonly IWebDriver driver;
        private string? courseName;

        public LanguageCourseFrontendTests()
        {
            var options = new FirefoxOptions();
            /* if (!string.Equals(Environment.GetEnvironmentVariable("SELENIUM_HEADLESS"),
                    "false", StringComparison.OrdinalIgnoreCase))
            {
                options.AddArgument("-headless");
            } */

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

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [Fact]
        public void Should_CreateLanguageCourse_WhenCorrectDataIsSavedAndRowIsInIndex()
        {
            courseName = $"Automaattestimise kursus {Guid.NewGuid():N}";

            /* tegevus */
            CreateLanguageCourse(courseName);

            /* kontroll */
            //kogume andmed
            IWebElement? row = FindCourseRow(courseName);
            Assert.NotNull(row);
            string languageCourseNameData = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
            string languageCourseKeelData = row.FindElement(By.Id("IndexTableCourseKeel")).Text;
            //assertime
            Assert.Equal(courseName, languageCourseNameData);
            Assert.Equal(CourseKeel, languageCourseKeelData);
        }

        [Fact]
        public void Should_ShowLanguageCourseDetails_WhenDetailsButtonIsClicked()
        {
            courseName = $"Automaattestimise kursus {Guid.NewGuid():N}";
            CreateLanguageCourse(courseName);

            /* tegevus */
            IWebElement? row = FindCourseRow(courseName);
            Assert.NotNull(row);
            IWebElement indexDetailsButton = row.FindElement(By.Id("IndexTableCourseDetails_BTN"));
            indexDetailsButton.Click();

            /* kontroll */
            //kogume andmed
            string nameData = driver.FindElement(By.Id("DetailsDelete_LanguageCourse_Nimetus")).Text;
            string keelData = driver.FindElement(By.Id("DetailsDelete_LanguageCourse_Keel")).Text;
            string taseData = driver.FindElement(By.Id("DetailsDelete_LanguageCourse_Tase")).Text;
            string kirjeldusData = driver.FindElement(By.Id("DetailsDelete_LanguageCourse_Kirjeldus")).Text;
            //assertime
            Assert.Equal(courseName, nameData);
            Assert.Equal(CourseKeel, keelData);
            Assert.Equal("3", taseData);
            Assert.Equal("Kursusel õpime automaattestimist.", kirjeldusData);
        }

        [Fact]
        public void Should_UpdateLanguageCourseName_WhenChangeIsSavedButNotWhenCancelled()
        {
            string oldName = $"Automaattestimise kursus {Guid.NewGuid():N}";
            string updatedName = oldName + " uuendatud";
            courseName = oldName;
            CreateLanguageCourse(oldName);

            /* tegevus 1: tühistame muudatuse */
            IWebElement? row = FindCourseRow(oldName);
            Assert.NotNull(row);
            IWebElement indexUpdateButton = row.FindElement(By.Id("IndexTableCourseUpdate_BTN"));
            indexUpdateButton.Click();
            IWebElement updateEntryNimetus = driver.FindElement(By.Id("CU_LanguageCourse_Nimetus"));
            Assert.Equal(oldName, updateEntryNimetus.GetDomProperty("value")); //vorm on eeltäidetud
            updateEntryNimetus.Clear();
            updateEntryNimetus.SendKeys(oldName + " salvestamata");
            IWebElement updateCancelButton = driver.FindElement(By.Id("CU_LanguageCourse_Cancel_BTN"));
            updateCancelButton.Click();

            /* kontroll 1 */
            Assert.NotNull(FindCourseRow(oldName));
            Assert.Null(FindCourseRow(oldName + " salvestamata"));

            /* tegevus 2: salvestame muudatuse */
            row = FindCourseRow(oldName);
            Assert.NotNull(row);
            indexUpdateButton = row.FindElement(By.Id("IndexTableCourseUpdate_BTN"));
            indexUpdateButton.Click();
            updateEntryNimetus = driver.FindElement(By.Id("CU_LanguageCourse_Nimetus"));
            updateEntryNimetus.Clear();
            updateEntryNimetus.SendKeys(updatedName);
            courseName = updatedName;
            IWebElement updateSaveButton = driver.FindElement(By.Id("CU_LanguageCourse_Save_BTN"));
            updateSaveButton.Click();

            /* kontroll 2 */
            //kogume andmed
            row = FindCourseRow(updatedName);
            Assert.NotNull(row);
            string languageCourseNameData = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
            string languageCourseKeelData = row.FindElement(By.Id("IndexTableCourseKeel")).Text;
            //assertime
            Assert.Equal(updatedName, languageCourseNameData);
            Assert.Equal(CourseKeel, languageCourseKeelData);
            Assert.Null(FindCourseRow(oldName));
        }

        [Fact]
        public void Should_DeleteLanguageCourse_WhenDeleteIsConfirmedAndRowIsGoneFromIndex()
        {
            string name = $"Automaattestimise kursus {Guid.NewGuid():N}";
            courseName = name;
            CreateLanguageCourse(name);

            /* tegevus */
            IWebElement? row = FindCourseRow(name);
            Assert.NotNull(row);
            IWebElement indexDeleteButton = row.FindElement(By.Id("IndexTableCourseDelete_BTN"));
            indexDeleteButton.Click();
            IWebElement deleteConfirmButton = driver.FindElement(By.Id("DetailsDelete_Delete_BTN"));
            deleteConfirmButton.Click();
            courseName = null; //kursus on juba kustutatud, puhastada pole vaja

            /* kontroll */
            Assert.Null(FindCourseRow(name));
        }

        public void Dispose()
        {
            try
            {
                if (courseName != null)
                {
                    DeleteLanguageCourse(courseName);
                }
            }
            finally
            {
                driver.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        private void CreateLanguageCourse(string name)
        {
            driver.Url = BaseUrl;
            IWebElement navigateToLanguageCourse = driver.FindElement(By.LinkText("Keelekursused")); //otsime testitava osa lingi
            navigateToLanguageCourse.Click(); //simuleeritud tegevus elemendiga, antud juhul klikk

            IWebElement indexCreateButton = driver.FindElement(By.Id("IndexCreateNewLanguageCourse_BTN"));
            indexCreateButton.Click();
            IWebElement createEntryNimetus = driver.FindElement(By.Id("CU_LanguageCourse_Nimetus"));
            createEntryNimetus.SendKeys(name);
            IWebElement createEntryKeel = driver.FindElement(By.Id("CU_LanguageCourse_Keel"));
            createEntryKeel.SendKeys(CourseKeel);
            IWebElement createEntryTase = driver.FindElement(By.Id("CU_LanguageCourse_Tase"));
            createEntryTase.SendKeys("3");
            IWebElement createEntryKirjeldus = driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus"));
            createEntryKirjeldus.SendKeys("Kursusel õpime automaattestimist.");
            IWebElement createSaveButton = driver.FindElement(By.Id("CU_LanguageCourse_Create_BTN"));
            createSaveButton.Click();
        }

        private void DeleteLanguageCourse(string name)
        {
            driver.Url = BaseUrl + "LanguageCourses";
            IWebElement? row = FindCourseRow(name);
            if (row == null)
            {
                return; //kursust pole salvestatud, midagi muud me ei kustuta
            }

            IWebElement indexDeleteButton = row.FindElement(By.Id("IndexTableCourseDelete_BTN"));
            indexDeleteButton.Click();
            IWebElement deleteConfirmButton = driver.FindElement(By.Id("DetailsDelete_Delete_BTN"));
            deleteConfirmButton.Click();
        }

        // otsime indeksi tabelist rea, mille nimetus ja keel klapivad, tagastab null kui rida pole
        private IWebElement? FindCourseRow(string name)
        {
            IWebElement tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0); //päiserida

            foreach (var row in rowsOfTable)
            {
                var nameElement = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
                var keelElement = row.FindElement(By.Id("IndexTableCourseKeel")).Text;
                if (nameElement == name && keelElement == CourseKeel)
                {
                    return row;
                }
            }

            return null;
        }
    }
}
