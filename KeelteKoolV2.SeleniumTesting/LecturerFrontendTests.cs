using OpenQA.Selenium;
using Xunit;

namespace KeelteKoolV2.SeleniumTesting
{
    [TestCaseOrderer("KeelteKoolV2.SeleniumTesting.PriorityOrderer", "KeelteKoolV2.SeleniumTesting")]
    public class LecturerFrontendTests : SeleniumTestBase
    {
        // indeks näitab 20 õppejõudu perekonnanime järgi sorteeritult, numbriga algav nimi hoiab testõppejõud nähtaval
        private const string LecturerLastName = "0 Selenium";
        private const string LecturerQualifications = "Automaattestimise õpetaja";
        // teine andmekomplekt, millega uuendustest kõik väljad üle kirjutab
        private const string UpdatedLecturerLastName = "0 Selenium uuendatud";
        private const string UpdatedLecturerQualifications = "Manuaaltestimise õpetaja";

        private string? lecturerFirstName;

        [Fact, TestPriority(1)]
        public void Should_CreateLecturer_WithCorrectData_ShowInIndex()
        {
            // Loob vormi kaudu uue õppejõu (eesnimi, perekonnanimi, kvalifikatsioon, pildita) ja salvestab selle.
            // Kontrollib, et pärast salvestamist suunati õppejõudude indeksisse (aadress ja pealkiri)
            // ning et tabelis on uus rida, mille veerud näitavad täpselt sisestatud eesnime ja perekonnanime.

            /* tegevus */
            lecturerFirstName = DataEntry();

            /* kontroll */
            //kogume andmed
            string headingData = driver.FindElement(By.Id("IndexHeading")).Text;
            IWebElement? row = FindLecturerRow(lecturerFirstName);
            Assert.NotNull(row);
            string lecturerFirstNameData = row.FindElement(By.Id("IndexTableLecturerFirstName")).Text;
            string lecturerLastNameData = row.FindElement(By.Id("IndexTableLecturerLastName")).Text;
            //assertime
            Assert.Equal(BaseUrl + "Lecturers", driver.Url);
            Assert.Equal("Õppejõud", headingData);
            Assert.Equal(lecturerFirstName, lecturerFirstNameData);
            Assert.Equal(LecturerLastName, lecturerLastNameData);

            DataClear(lecturerFirstName);
        }

        [Fact, TestPriority(2)]
        public void Should_ReadLecturer_ShowDetails()
        {
            // Loob samamoodi nagu loomistestis uue õppejõu ja avab indeksist selle õppejõu andmete lehe.
            // Kontrollib lehe pealkirju ja aadressi ning et iga välja väärtus (Id, eesnimi, perekonnanimi,
            // kvalifikatsioon) on täpselt see, mis loomisel sisestati. Lõpuks kontrollib, et "Tagasi" viib indeksisse.
            lecturerFirstName = DataEntry();

            /* tegevus */
            IWebElement? row = FindLecturerRow(lecturerFirstName);
            Assert.NotNull(row);
            IWebElement indexDetailsButton = row.FindElement(By.Id("IndexTableLecturerDetails_BTN"));
            indexDetailsButton.Click();

            /* kontroll */
            //kogume andmed
            string headingData = driver.FindElement(By.Id("DetailsDelete_Heading")).Text;
            string subHeadingData = driver.FindElement(By.Id("DetailsDelete_SubHeading")).Text;
            string idData = driver.FindElement(By.Id("DetailsDelete_Lecturer_Id")).Text;
            string firstNameData = driver.FindElement(By.Id("DetailsDelete_Lecturer_FirstName")).Text;
            string lastNameData = driver.FindElement(By.Id("DetailsDelete_Lecturer_LastName")).Text;
            string qualificationsData = driver.FindElement(By.Id("DetailsDelete_Lecturer_Qualifications")).Text;
            //assertime
            Assert.Equal("Andmed", headingData);
            Assert.Equal("Õppejõud", subHeadingData);
            Assert.True(Guid.TryParse(idData, out _));
            Assert.Equal(BaseUrl + "Lecturers/Details/" + idData, driver.Url); //leht on just selle õppejõu oma
            Assert.Equal(lecturerFirstName, firstNameData);
            Assert.Equal(LecturerLastName, lastNameData);
            Assert.Equal(LecturerQualifications, qualificationsData);

            /* tegevus 2: tagasi indeksisse */
            IWebElement detailsBackButton = driver.FindElement(By.Id("DetailsDelete_Back_BTN"));
            detailsBackButton.Click();

            /* kontroll 2 */
            Assert.Equal(BaseUrl + "Lecturers", driver.Url);
            Assert.NotNull(FindLecturerRow(lecturerFirstName));

            DataClear(lecturerFirstName);
        }

        [Fact, TestPriority(3)]
        public void Should_UpdateLecturer_ShowNewValues_KeepSameRecord()
        {
            // Loob uue õppejõu, avab indeksist sama õppejõu muutmisvormi ja kirjutab üle kõik kolm välja
            // (eesnimi, perekonnanimi, kvalifikatsioon) ning salvestab. Kontrollib, et indeks ja andmete leht
            // näitavad uusi väärtusi, vanu väärtusi enam ei näidata ning et see on sama kirje:
            // sama Id aadressis ja andmete lehel ning indeksi ridade arv ei muutunud.
            string oldFirstName = DataEntry();
            string updatedFirstName = oldFirstName + " uuendatud";
            int rowCountBefore = driver.FindElement(By.Id("IndexTable")).FindElements(By.TagName("tr")).Count;

            /* tegevus */
            IWebElement? row = FindLecturerRow(oldFirstName);
            Assert.NotNull(row);
            IWebElement indexUpdateButton = row.FindElement(By.Id("IndexTableLecturerUpdate_BTN"));
            indexUpdateButton.Click();
            string lecturerId = driver.Url.Substring((BaseUrl + "Lecturers/Update/").Length); //muudetava õppejõu Id aadressist
            Assert.True(Guid.TryParse(lecturerId, out _));
            IWebElement updateEntryFirstName = driver.FindElement(By.Id("CU_Lecturer_FirstName"));
            Assert.Equal(oldFirstName, updateEntryFirstName.GetDomProperty("value")); //vorm on eeltäidetud
            updateEntryFirstName.Clear();
            updateEntryFirstName.SendKeys(updatedFirstName);
            lecturerFirstName = updatedFirstName;
            IWebElement updateEntryLastName = driver.FindElement(By.Id("CU_Lecturer_LastName"));
            Assert.Equal(LecturerLastName, updateEntryLastName.GetDomProperty("value"));
            updateEntryLastName.Clear();
            updateEntryLastName.SendKeys(UpdatedLecturerLastName);
            IWebElement updateEntryQualifications = driver.FindElement(By.Id("CU_Lecturer_Qualifications"));
            Assert.Equal(LecturerQualifications, updateEntryQualifications.GetDomProperty("value"));
            updateEntryQualifications.Clear();
            updateEntryQualifications.SendKeys(UpdatedLecturerQualifications);
            IWebElement updateSaveButton = driver.FindElement(By.Id("CU_Lecturer_Save_BTN"));
            updateSaveButton.Click();

            /* kontroll 1: indeks */
            //kogume andmed
            string headingData = driver.FindElement(By.Id("IndexHeading")).Text;
            int rowCountAfter = driver.FindElement(By.Id("IndexTable")).FindElements(By.TagName("tr")).Count;
            row = FindLecturerRow(updatedFirstName);
            Assert.NotNull(row);
            string lecturerFirstNameData = row.FindElement(By.Id("IndexTableLecturerFirstName")).Text;
            string lecturerLastNameData = row.FindElement(By.Id("IndexTableLecturerLastName")).Text;
            //assertime
            Assert.Equal(BaseUrl + "Lecturers", driver.Url);
            Assert.Equal("Õppejõud", headingData);
            Assert.Equal(updatedFirstName, lecturerFirstNameData);
            Assert.Equal(UpdatedLecturerLastName, lecturerLastNameData);
            Assert.Null(FindLecturerRow(oldFirstName)); //vana nimega rida pole enam
            Assert.Equal(rowCountBefore, rowCountAfter); //uut rida ei tekkinud

            /* kontroll 2: andmete leht */
            IWebElement indexDetailsButton = row.FindElement(By.Id("IndexTableLecturerDetails_BTN"));
            indexDetailsButton.Click();
            string idData = driver.FindElement(By.Id("DetailsDelete_Lecturer_Id")).Text;
            string firstNameData = driver.FindElement(By.Id("DetailsDelete_Lecturer_FirstName")).Text;
            string lastNameData = driver.FindElement(By.Id("DetailsDelete_Lecturer_LastName")).Text;
            string qualificationsData = driver.FindElement(By.Id("DetailsDelete_Lecturer_Qualifications")).Text;
            Assert.Equal(BaseUrl + "Lecturers/Details/" + lecturerId, driver.Url); //sama kirje
            Assert.Equal(lecturerId, idData);
            Assert.Equal(updatedFirstName, firstNameData);
            Assert.Equal(UpdatedLecturerLastName, lastNameData);
            Assert.Equal(UpdatedLecturerQualifications, qualificationsData);
            Assert.NotEqual(oldFirstName, firstNameData);
            Assert.NotEqual(LecturerLastName, lastNameData);
            Assert.NotEqual(LecturerQualifications, qualificationsData);

            DataClear(updatedFirstName);
        }

        protected override void CleanUp()
        {
            if (lecturerFirstName != null)
            {
                DeleteLecturer(lecturerFirstName);
            }
        }

        // sisestab unikaalse eesnimega testõppejõu ja tagastab selle eesnime, Dispose puhastab õppejõu ka siis, kui test kukub
        private string DataEntry()
        {
            string firstName = UniqueValue("Automaattestija ");
            lecturerFirstName = firstName;
            CreateLecturer(firstName);
            return firstName;
        }

        // kustutab testõppejõu, et järgmine test saaks puhtalt alustada
        private void DataClear(string firstName)
        {
            DeleteLecturer(firstName);
            if (lecturerFirstName == firstName)
            {
                lecturerFirstName = null;
            }
        }

        private void CreateLecturer(string firstName)
        {
            GoTo("");
            IWebElement navigateToLecturers = driver.FindElement(By.LinkText("Õppejõud")); //otsime testitava osa lingi
            navigateToLecturers.Click();

            IWebElement indexCreateButton = driver.FindElement(By.Id("IndexCreateNewLecturer_BTN"));
            indexCreateButton.Click();
            IWebElement createEntryFirstName = driver.FindElement(By.Id("CU_Lecturer_FirstName"));
            createEntryFirstName.SendKeys(firstName);
            IWebElement createEntryLastName = driver.FindElement(By.Id("CU_Lecturer_LastName"));
            createEntryLastName.SendKeys(LecturerLastName);
            IWebElement createEntryQualifications = driver.FindElement(By.Id("CU_Lecturer_Qualifications"));
            createEntryQualifications.SendKeys(LecturerQualifications);
            IWebElement createSaveButton = driver.FindElement(By.Id("CU_Lecturer_Create_BTN"));
            createSaveButton.Click();
        }

        private void DeleteLecturer(string firstName)
        {
            GoTo("Lecturers");
            IWebElement? row = FindLecturerRow(firstName);
            if (row == null)
            {
                return; //õppejõudu pole salvestatud, midagi muud me ei kustuta
            }

            IWebElement indexDeleteButton = row.FindElement(By.Id("IndexTableLecturerDelete_BTN"));
            indexDeleteButton.Click();
            IWebElement deleteConfirmButton = driver.FindElement(By.Id("DetailsDelete_Delete_BTN"));
            deleteConfirmButton.Click();
        }

        // otsime indeksi tabelist rea, mille eesnimi ja (test)perekonnanimi klapivad, tagastab null kui rida pole
        private IWebElement? FindLecturerRow(string firstName)
        {
            IWebElement tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0); //päiserida

            foreach (var row in rowsOfTable)
            {
                var firstNameElement = row.FindElement(By.Id("IndexTableLecturerFirstName")).Text;
                var lastNameElement = row.FindElement(By.Id("IndexTableLecturerLastName")).Text;
                if (firstNameElement == firstName && lastNameElement is LecturerLastName or UpdatedLecturerLastName)
                {
                    return row;
                }
            }

            return null;
        }
    }
}
