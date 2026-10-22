using OpenQA.Selenium;
using Xunit;

namespace KeelteKoolV2.SeleniumTesting
{
    [TestCaseOrderer("KeelteKoolV2.SeleniumTesting.PriorityOrderer", "KeelteKoolV2.SeleniumTesting")]
    public class LanguageCourseFrontendTests : SeleniumTestBase
    {
        // indeks näitab 20 kursust Keele järgi sorteeritult, numbriga algav keel hoiab testkursused nähtaval
        private const string CourseKeel = "0 Selenium";
        // teine andmekomplekt, millega uuendustest kõik väljad üle kirjutab
        private const string UpdatedCourseKeel = "0 Mantis, Excel";
        private const string UpdatedCourseTase = "4";
        private const string UpdatedCourseKirjeldus = "Kursusel õpime manuaaltestimist.";

        private string? courseName;

        [Fact, TestPriority(1)]
        public void Should_CreateLanguageCourse_WithCorrectData_ShowInIndex()
        {
            /* tegevus */
            courseName = DataEntry();

            /* kontroll */
            //kogume andmed
            IWebElement? row = FindCourseRow(courseName);
            Assert.NotNull(row);
            string languageCourseNameData = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
            string languageCourseKeelData = row.FindElement(By.Id("IndexTableCourseKeel")).Text;
            //assertime
            Assert.Equal(courseName, languageCourseNameData);
            Assert.Equal(CourseKeel, languageCourseKeelData);

            DataClear(courseName);
        }

        [Fact, TestPriority(2)]
        public void Should_ReadLanguageCourse_ShowDetails()
        {
            courseName = DataEntry();

            /* tegevus */
            IWebElement? row = FindCourseRow(courseName);
            Assert.NotNull(row);
            IWebElement indexDetailsButton = row.FindElement(By.Id("IndexTableCourseDetails_BTN"));
            indexDetailsButton.Click();

            /* kontroll */
            //kogume andmed
            string idData = driver.FindElement(By.Id("DetailsDelete_LanguageCourse_Id")).Text;
            string nameData = driver.FindElement(By.Id("DetailsDelete_LanguageCourse_Nimetus")).Text;
            string keelData = driver.FindElement(By.Id("DetailsDelete_LanguageCourse_Keel")).Text;
            string taseData = driver.FindElement(By.Id("DetailsDelete_LanguageCourse_Tase")).Text;
            string kirjeldusData = driver.FindElement(By.Id("DetailsDelete_LanguageCourse_Kirjeldus")).Text;
            //assertime
            Assert.NotEmpty(idData);
            Assert.True(Guid.TryParse(idData, out _));
            Assert.Equal(courseName, nameData);
            Assert.Equal(CourseKeel, keelData);
            Assert.Equal("3", taseData);
            Assert.True(int.Parse(taseData) > -1);
            Assert.Equal("Kursusel õpime automaattestimist.", kirjeldusData);

            /* tegevus 2: tagasi indeksisse */
            IWebElement detailsBackButton = driver.FindElement(By.Id("DetailsDelete_Back_BTN"));
            detailsBackButton.Click();

            /* kontroll 2 */
            Assert.NotNull(FindCourseRow(courseName));

            DataClear(courseName);
        }

        [Fact, TestPriority(3)]
        public void Should_UpdateLanguageCourse_CancelKeepsOriginal_SaveChangesName()
        {
            string oldName = DataEntry();
            string updatedName = oldName + " uuendatud";

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
            //kirjutame üle ka ülejäänud väljad teise andmekomplektiga
            IWebElement updateEntryKeel = driver.FindElement(By.Id("CU_LanguageCourse_Keel"));
            updateEntryKeel.Clear();
            updateEntryKeel.SendKeys(UpdatedCourseKeel);
            IWebElement updateEntryTase = driver.FindElement(By.Id("CU_LanguageCourse_Tase"));
            updateEntryTase.Clear();
            updateEntryTase.SendKeys(UpdatedCourseTase);
            IWebElement updateEntryKirjeldus = driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus"));
            updateEntryKirjeldus.Clear();
            updateEntryKirjeldus.SendKeys(UpdatedCourseKirjeldus);
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
            Assert.Equal(UpdatedCourseKeel, languageCourseKeelData);
            Assert.Null(FindCourseRow(oldName));

            /* kontroll 3: uuendatud andmed on ka detailvaates */
            IWebElement indexDetailsButton = row.FindElement(By.Id("IndexTableCourseDetails_BTN"));
            indexDetailsButton.Click();
            string idData = driver.FindElement(By.Id("DetailsDelete_LanguageCourse_Id")).Text;
            string nameData = driver.FindElement(By.Id("DetailsDelete_LanguageCourse_Nimetus")).Text;
            string keelData = driver.FindElement(By.Id("DetailsDelete_LanguageCourse_Keel")).Text;
            string taseData = driver.FindElement(By.Id("DetailsDelete_LanguageCourse_Tase")).Text;
            string kirjeldusData = driver.FindElement(By.Id("DetailsDelete_LanguageCourse_Kirjeldus")).Text;
            Assert.NotEmpty(idData);
            Assert.Equal(updatedName, nameData);
            Assert.Equal(UpdatedCourseKeel, keelData);
            Assert.Equal(UpdatedCourseTase, taseData);
            Assert.Equal(UpdatedCourseKirjeldus, kirjeldusData);
            Assert.NotEqual(CourseKeel, keelData);
            Assert.NotEqual("3", taseData);
            Assert.NotEqual("Kursusel õpime automaattestimist.", kirjeldusData);

            DataClear(updatedName);
        }

        [Fact, TestPriority(4)]
        public void Should_DeleteLanguageCourse_RemoveFromIndex()
        {
            string name = DataEntry();

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

        protected override void CleanUp()
        {
            if (courseName != null)
            {
                DeleteLanguageCourse(courseName);
            }
        }

        // sisestab unikaalse nimega testkursuse ja tagastab selle nime, Dispose puhastab kursuse ka siis, kui test kukub
        private string DataEntry()
        {
            string name = UniqueValue("Automaattestimise kursus ");
            courseName = name;
            CreateLanguageCourse(name);
            return name;
        }

        // kustutab testkursuse, et järgmine test saaks puhtalt alustada
        private void DataClear(string name)
        {
            DeleteLanguageCourse(name);
            if (courseName == name)
            {
                courseName = null;
            }
        }

        private void CreateLanguageCourse(string name)
        {
            GoTo("");
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
            GoTo("LanguageCourses");
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

        // otsime indeksi tabelist rea, mille nimetus ja (testi)keel klapivad, tagastab null kui rida pole
        private IWebElement? FindCourseRow(string name)
        {
            IWebElement tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0); //päiserida

            foreach (var row in rowsOfTable)
            {
                var nameElement = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
                var keelElement = row.FindElement(By.Id("IndexTableCourseKeel")).Text;
                if (nameElement == name && keelElement is CourseKeel or UpdatedCourseKeel)
                {
                    return row;
                }
            }

            return null;
        }
    }
}
