using OpenQA.Selenium;
using Xunit;

namespace KeelteKoolV2.SeleniumTesting
{
    [TestCaseOrderer("KeelteKoolV2.SeleniumTesting.PriorityOrderer", "KeelteKoolV2.SeleniumTesting")]
    public class StudentFrontendTests : SeleniumTestBase
    {
        // indeks näitab 20 õpilast perekonnanime järgi sorteeritult, numbriga algav nimi hoiab testõpilased nähtaval
        private const string StudentFirstName = "Automaattestija";
        private const string StudentLastName = "0 Selenium";
        // kuupäevaväli võtab väärtuse kujul yyyy-MM-dd, lehel näidatakse seda kujul dd.MM.yyyy
        private const string StudentDateOfBirthInput = "2005-03-15";
        private const string StudentDateOfBirthShown = "15.03.2005";
        // teine andmekomplekt, millega uuendustest perekonnanime ja sünniaja üle kirjutab
        private const string UpdatedStudentLastName = "0 Selenium uuendatud";
        private const string UpdatedStudentDateOfBirthInput = "2006-07-20";
        private const string UpdatedStudentDateOfBirthShown = "20.07.2006";
        // sünniaja testis muudetakse ainult eesnime
        private const string UpdatedStudentFirstName = "Automaattestija uuendatud";
        // e-post ilma @ märgita, mille brauser peab tagasi lükkama
        private const string InvalidStudentEmail = "maksim.at.mail";

        private string? studentEmail;

        [Fact, TestPriority(1)]
        public void Should_CreateStudent_WithCorrectData_ShowInIndex()
        {
            // Loob vormi kaudu uue õpilase (eesnimi, perekonnanimi, unikaalne e-post, sünniaeg) ja salvestab selle.
            // Kontrollib, et pärast salvestamist suunati õpilaste indeksisse (aadress ja pealkiri)
            // ning et tabelis on uus rida, mille veerud näitavad täpselt sisestatud andmeid, ka sünniaega õigel kujul.

            /* tegevus */
            studentEmail = DataEntry();

            /* kontroll */
            //kogume andmed
            string headingData = driver.FindElement(By.Id("IndexHeading")).Text;
            IWebElement? row = FindStudentRow(studentEmail);
            Assert.NotNull(row);
            string studentFirstNameData = row.FindElement(By.Id("IndexTableStudentFirstName")).Text;
            string studentLastNameData = row.FindElement(By.Id("IndexTableStudentLastName")).Text;
            string studentEmailData = row.FindElement(By.Id("IndexTableStudentEmail")).Text;
            string studentDateOfBirthData = row.FindElement(By.Id("IndexTableStudentDateOfBirth")).Text;
            //assertime
            Assert.Equal(BaseUrl + "Students", driver.Url);
            Assert.Equal("Õpilased", headingData);
            Assert.Equal(StudentFirstName, studentFirstNameData);
            Assert.Equal(StudentLastName, studentLastNameData);
            Assert.Equal(studentEmail, studentEmailData);
            Assert.Equal(StudentDateOfBirthShown, studentDateOfBirthData);

            DataClear(studentEmail);
        }

        [Fact, TestPriority(2)]
        public void Should_ReadStudent_ShowDetails()
        {
            // Loob samamoodi nagu loomistestis uue õpilase ja avab indeksist selle õpilase andmete lehe.
            // Kontrollib lehe pealkirju ja aadressi ning et iga välja väärtus (Id, eesnimi, perekonnanimi,
            // e-post, sünniaeg) on täpselt see, mis loomisel sisestati. Lõpuks kontrollib, et "Tagasi" viib indeksisse.
            studentEmail = DataEntry();

            /* tegevus */
            IWebElement? row = FindStudentRow(studentEmail);
            Assert.NotNull(row);
            IWebElement indexDetailsButton = row.FindElement(By.Id("IndexTableStudentDetails_BTN"));
            indexDetailsButton.Click();

            /* kontroll */
            //kogume andmed
            string headingData = driver.FindElement(By.Id("DetailsDelete_Heading")).Text;
            string subHeadingData = driver.FindElement(By.Id("DetailsDelete_SubHeading")).Text;
            string idData = driver.FindElement(By.Id("DetailsDelete_Student_Id")).Text;
            string firstNameData = driver.FindElement(By.Id("DetailsDelete_Student_FirstName")).Text;
            string lastNameData = driver.FindElement(By.Id("DetailsDelete_Student_LastName")).Text;
            string emailData = driver.FindElement(By.Id("DetailsDelete_Student_Email")).Text;
            string dateOfBirthData = driver.FindElement(By.Id("DetailsDelete_Student_DateOfBirth")).Text;
            //assertime
            Assert.Equal("Andmed", headingData);
            Assert.Equal("Õpilane", subHeadingData);
            Assert.True(Guid.TryParse(idData, out _));
            Assert.Equal(BaseUrl + "Students/Details/" + idData, driver.Url); //leht on just selle õpilase oma
            Assert.Equal(StudentFirstName, firstNameData);
            Assert.Equal(StudentLastName, lastNameData);
            Assert.Equal(studentEmail, emailData);
            Assert.Equal(StudentDateOfBirthShown, dateOfBirthData);

            /* tegevus 2: tagasi indeksisse */
            IWebElement detailsBackButton = driver.FindElement(By.Id("DetailsDelete_Back_BTN"));
            detailsBackButton.Click();

            /* kontroll 2 */
            Assert.Equal(BaseUrl + "Students", driver.Url);
            Assert.NotNull(FindStudentRow(studentEmail));

            DataClear(studentEmail);
        }

        [Fact, TestPriority(3)]
        public void Should_UpdateStudent_ShowNewValues_KeepSameRecord()
        {
            // Loob uue õpilase, avab indeksist sama õpilase muutmisvormi ja kirjutab üle e-posti, perekonnanime
            // ja sünniaja ning salvestab. Kontrollib, et indeks ja andmete leht näitavad uusi väärtusi,
            // vanu väärtusi enam ei näidata ning et see on sama kirje: sama Id aadressis ja andmete lehel
            // ning indeksi ridade arv ei muutunud.
            string oldEmail = DataEntry();
            string updatedEmail = UniqueValue("uuendatud") + "@selenium.ee";
            int rowCountBefore = driver.FindElement(By.Id("IndexTable")).FindElements(By.TagName("tr")).Count;

            /* tegevus */
            IWebElement? row = FindStudentRow(oldEmail);
            Assert.NotNull(row);
            IWebElement indexUpdateButton = row.FindElement(By.Id("IndexTableStudentUpdate_BTN"));
            indexUpdateButton.Click();
            string studentId = driver.Url.Substring((BaseUrl + "Students/Update/").Length); //muudetava õpilase Id aadressist
            Assert.True(Guid.TryParse(studentId, out _));
            IWebElement updateEntryFirstName = driver.FindElement(By.Id("CU_Student_FirstName"));
            Assert.Equal(StudentFirstName, updateEntryFirstName.GetDomProperty("value")); //vorm on eeltäidetud
            IWebElement updateEntryLastName = driver.FindElement(By.Id("CU_Student_LastName"));
            Assert.Equal(StudentLastName, updateEntryLastName.GetDomProperty("value"));
            updateEntryLastName.Clear();
            updateEntryLastName.SendKeys(UpdatedStudentLastName);
            IWebElement updateEntryEmail = driver.FindElement(By.Id("CU_Student_Email"));
            Assert.Equal(oldEmail, updateEntryEmail.GetDomProperty("value"));
            updateEntryEmail.Clear();
            updateEntryEmail.SendKeys(updatedEmail);
            studentEmail = updatedEmail;
            IWebElement updateEntryDateOfBirth = driver.FindElement(By.Id("CU_Student_DateOfBirth"));
            Assert.Equal(StudentDateOfBirthInput, updateEntryDateOfBirth.GetDomProperty("value"));
            //kuupäevavälja klahvidega täitmine sõltub brauseri keelest, seega paneme väärtuse otse
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].value = arguments[1];", updateEntryDateOfBirth, UpdatedStudentDateOfBirthInput);
            IWebElement updateSaveButton = driver.FindElement(By.Id("CU_Student_Save_BTN"));
            updateSaveButton.Click();

            /* kontroll 1: indeks */
            //kogume andmed
            string headingData = driver.FindElement(By.Id("IndexHeading")).Text;
            int rowCountAfter = driver.FindElement(By.Id("IndexTable")).FindElements(By.TagName("tr")).Count;
            row = FindStudentRow(updatedEmail);
            Assert.NotNull(row);
            string studentFirstNameData = row.FindElement(By.Id("IndexTableStudentFirstName")).Text;
            string studentLastNameData = row.FindElement(By.Id("IndexTableStudentLastName")).Text;
            string studentEmailData = row.FindElement(By.Id("IndexTableStudentEmail")).Text;
            string studentDateOfBirthData = row.FindElement(By.Id("IndexTableStudentDateOfBirth")).Text;
            //assertime
            Assert.Equal(BaseUrl + "Students", driver.Url);
            Assert.Equal("Õpilased", headingData);
            Assert.Equal(StudentFirstName, studentFirstNameData);
            Assert.Equal(UpdatedStudentLastName, studentLastNameData);
            Assert.Equal(updatedEmail, studentEmailData);
            Assert.Equal(UpdatedStudentDateOfBirthShown, studentDateOfBirthData);
            Assert.Null(FindStudentRow(oldEmail)); //vana e-postiga rida pole enam
            Assert.Equal(rowCountBefore, rowCountAfter); //uut rida ei tekkinud

            /* kontroll 2: andmete leht */
            IWebElement indexDetailsButton = row.FindElement(By.Id("IndexTableStudentDetails_BTN"));
            indexDetailsButton.Click();
            string idData = driver.FindElement(By.Id("DetailsDelete_Student_Id")).Text;
            string firstNameData = driver.FindElement(By.Id("DetailsDelete_Student_FirstName")).Text;
            string lastNameData = driver.FindElement(By.Id("DetailsDelete_Student_LastName")).Text;
            string emailData = driver.FindElement(By.Id("DetailsDelete_Student_Email")).Text;
            string dateOfBirthData = driver.FindElement(By.Id("DetailsDelete_Student_DateOfBirth")).Text;
            Assert.Equal(BaseUrl + "Students/Details/" + studentId, driver.Url); //sama kirje
            Assert.Equal(studentId, idData);
            Assert.Equal(StudentFirstName, firstNameData);
            Assert.Equal(UpdatedStudentLastName, lastNameData);
            Assert.Equal(updatedEmail, emailData);
            Assert.Equal(UpdatedStudentDateOfBirthShown, dateOfBirthData);
            Assert.NotEqual(StudentLastName, lastNameData);
            Assert.NotEqual(oldEmail, emailData);
            Assert.NotEqual(StudentDateOfBirthShown, dateOfBirthData);

            DataClear(updatedEmail);
        }

        [Fact, TestPriority(4)]
        public void Should_DeleteStudent_RemoveFromIndex_DetailsNotFound()
        {
            // Loob uue õpilase, avab indeksist sama õpilase kustutamise lehe ja kontrollib, et kinnitusleht
            // näitab just selle õpilase andmeid. Pärast kustutamise kinnitamist kontrollib, et ollakse tagasi
            // indeksis, õpilase rida on kadunud, ridu on ühe võrra vähem ja vana andmete aadress õpilast enam ei näita.
            string email = DataEntry();
            int rowCountBefore = driver.FindElement(By.Id("IndexTable")).FindElements(By.TagName("tr")).Count;

            /* tegevus 1: avame kustutamise lehe */
            IWebElement? row = FindStudentRow(email);
            Assert.NotNull(row);
            IWebElement indexDeleteButton = row.FindElement(By.Id("IndexTableStudentDelete_BTN"));
            indexDeleteButton.Click();

            /* kontroll 1: kinnitusleht näitab õpilase andmeid */
            //kogume andmed
            string studentId = driver.Url.Substring((BaseUrl + "Students/Delete/").Length); //kustutatava õpilase Id aadressist
            string headingData = driver.FindElement(By.Id("DetailsDelete_Heading")).Text;
            string subHeadingData = driver.FindElement(By.Id("DetailsDelete_SubHeading")).Text;
            string idData = driver.FindElement(By.Id("DetailsDelete_Student_Id")).Text;
            string firstNameData = driver.FindElement(By.Id("DetailsDelete_Student_FirstName")).Text;
            string lastNameData = driver.FindElement(By.Id("DetailsDelete_Student_LastName")).Text;
            string emailData = driver.FindElement(By.Id("DetailsDelete_Student_Email")).Text;
            string dateOfBirthData = driver.FindElement(By.Id("DetailsDelete_Student_DateOfBirth")).Text;
            //assertime
            Assert.True(Guid.TryParse(studentId, out _));
            Assert.Equal("Kustuta õpilane", headingData);
            Assert.Equal("Kas oled kindel, et soovid selle õpilase kustutada?", subHeadingData);
            Assert.Equal(studentId, idData);
            Assert.Equal(StudentFirstName, firstNameData);
            Assert.Equal(StudentLastName, lastNameData);
            Assert.Equal(email, emailData);
            Assert.Equal(StudentDateOfBirthShown, dateOfBirthData);

            /* tegevus 2: kinnitame kustutamise */
            IWebElement deleteConfirmButton = driver.FindElement(By.Id("DetailsDelete_Delete_BTN"));
            deleteConfirmButton.Click();
            studentEmail = null; //õpilane on juba kustutatud, puhastada pole vaja

            /* kontroll 2: indeks */
            string indexHeadingData = driver.FindElement(By.Id("IndexHeading")).Text;
            int rowCountAfter = driver.FindElement(By.Id("IndexTable")).FindElements(By.TagName("tr")).Count;
            Assert.Equal(BaseUrl + "Students", driver.Url);
            Assert.Equal("Õpilased", indexHeadingData);
            Assert.Null(FindStudentRow(email));
            Assert.Equal(rowCountBefore - 1, rowCountAfter);

            /* kontroll 3: vana andmete aadress õpilast enam ei näita */
            GoTo("Students/Details/" + studentId);
            Assert.DoesNotContain(studentId, driver.PageSource);
            Assert.DoesNotContain(email, driver.PageSource);
        }

        [Fact, TestPriority(5)]
        public void Should_NotCreateStudent_WithInvalidEmail_StayOnCreatePage()
        {
            // Täidab uue õpilase vormi korrektsete andmetega, aga vigase e-postiga ("maksim.at.mail") ja vajutab "Loo õpilane".
            // Kontrollib, et brauser ei saatnud vormi ära: ollakse endiselt loomise lehel (aadress ja pealkiri),
            // e-posti väli on vigane ja selle juures on valideerimisteade ning indeksisse ei tekkinud uut rida.
            GoTo("Students");
            int rowCountBefore = driver.FindElement(By.Id("IndexTable")).FindElements(By.TagName("tr")).Count;
            studentEmail = InvalidStudentEmail; //kui vorm siiski läheb läbi, kustutab CleanUp selle õpilase

            /* tegevus */
            IWebElement indexCreateButton = driver.FindElement(By.Id("IndexCreateNewStudent_BTN"));
            indexCreateButton.Click();
            IWebElement createEntryFirstName = driver.FindElement(By.Id("CU_Student_FirstName"));
            createEntryFirstName.SendKeys(StudentFirstName);
            IWebElement createEntryLastName = driver.FindElement(By.Id("CU_Student_LastName"));
            createEntryLastName.SendKeys(StudentLastName);
            IWebElement createEntryEmail = driver.FindElement(By.Id("CU_Student_Email"));
            createEntryEmail.SendKeys(InvalidStudentEmail);
            IWebElement createEntryDateOfBirth = driver.FindElement(By.Id("CU_Student_DateOfBirth"));
            //kuupäevavälja klahvidega täitmine sõltub brauseri keelest, seega paneme väärtuse otse
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].value = arguments[1];", createEntryDateOfBirth, StudentDateOfBirthInput);
            IWebElement createSaveButton = driver.FindElement(By.Id("CU_Student_Create_BTN"));
            createSaveButton.Click();

            /* kontroll 1: jäime loomise lehele ja e-posti väli on vigane */
            //kogume andmed
            string headingData = driver.FindElement(By.Id("CU_Heading")).Text;
            createEntryEmail = driver.FindElement(By.Id("CU_Student_Email"));
            string emailValueData = createEntryEmail.GetDomProperty("value");
            //brauseri teade sõltub brauseri keelest, seega kontrollime ainult, et teade on olemas
            string validationMessageData = createEntryEmail.GetDomProperty("validationMessage");
            bool emailTypeMismatch = (bool)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].validity.typeMismatch;", createEntryEmail);
            //assertime
            Assert.Equal(BaseUrl + "Students/Create", driver.Url);
            Assert.Equal("Loo õpilane", headingData);
            Assert.Equal(InvalidStudentEmail, emailValueData); //sisestatud väärtus on alles
            Assert.True(emailTypeMismatch);
            Assert.False(string.IsNullOrWhiteSpace(validationMessageData));

            /* kontroll 2: indeksisse ei tekkinud uut rida */
            GoTo("Students");
            int rowCountAfter = driver.FindElement(By.Id("IndexTable")).FindElements(By.TagName("tr")).Count;
            Assert.Null(FindStudentRow(InvalidStudentEmail));
            Assert.Equal(rowCountBefore, rowCountAfter);
        }

        [Fact, TestPriority(6)]
        public void Should_UpdateStudentFirstName_KeepDateOfBirth()
        {
            // Loob uue õpilase, avab tema muutmisvormi, muudab ainult eesnime ja salvestab.
            // Kontrollib, et sünniaeg on indeksis, andmete lehel ja uuesti avatud muutmisvormis täpselt sama
            // mis loomisel (ei läinud tühjaks ega nihkunud päeva võrra), eesnimi on uus ja teised väljad ei muutunud.
            string email = DataEntry();

            /* tegevus */
            IWebElement? row = FindStudentRow(email);
            Assert.NotNull(row);
            IWebElement indexUpdateButton = row.FindElement(By.Id("IndexTableStudentUpdate_BTN"));
            indexUpdateButton.Click();
            string studentId = driver.Url.Substring((BaseUrl + "Students/Update/").Length); //muudetava õpilase Id aadressist
            Assert.True(Guid.TryParse(studentId, out _));
            IWebElement updateEntryDateOfBirth = driver.FindElement(By.Id("CU_Student_DateOfBirth"));
            Assert.Equal(StudentDateOfBirthInput, updateEntryDateOfBirth.GetDomProperty("value")); //sünniaeg on vormis eeltäidetud
            IWebElement updateEntryFirstName = driver.FindElement(By.Id("CU_Student_FirstName"));
            updateEntryFirstName.Clear();
            updateEntryFirstName.SendKeys(UpdatedStudentFirstName);
            IWebElement updateSaveButton = driver.FindElement(By.Id("CU_Student_Save_BTN"));
            updateSaveButton.Click();

            /* kontroll 1: indeks */
            //kogume andmed
            row = FindStudentRow(email);
            Assert.NotNull(row);
            string studentFirstNameData = row.FindElement(By.Id("IndexTableStudentFirstName")).Text;
            string studentDateOfBirthData = row.FindElement(By.Id("IndexTableStudentDateOfBirth")).Text;
            //assertime
            Assert.Equal(BaseUrl + "Students", driver.Url);
            Assert.Equal(UpdatedStudentFirstName, studentFirstNameData);
            Assert.Equal(StudentDateOfBirthShown, studentDateOfBirthData);

            /* kontroll 2: andmete leht */
            IWebElement indexDetailsButton = row.FindElement(By.Id("IndexTableStudentDetails_BTN"));
            indexDetailsButton.Click();
            string idData = driver.FindElement(By.Id("DetailsDelete_Student_Id")).Text;
            string firstNameData = driver.FindElement(By.Id("DetailsDelete_Student_FirstName")).Text;
            string lastNameData = driver.FindElement(By.Id("DetailsDelete_Student_LastName")).Text;
            string emailData = driver.FindElement(By.Id("DetailsDelete_Student_Email")).Text;
            string dateOfBirthData = driver.FindElement(By.Id("DetailsDelete_Student_DateOfBirth")).Text;
            Assert.Equal(BaseUrl + "Students/Details/" + studentId, driver.Url); //sama kirje
            Assert.Equal(studentId, idData);
            Assert.Equal(UpdatedStudentFirstName, firstNameData);
            Assert.Equal(StudentLastName, lastNameData);
            Assert.Equal(email, emailData);
            Assert.Equal(StudentDateOfBirthShown, dateOfBirthData);

            /* kontroll 3: uuesti avatud muutmisvormis on sama sünniaeg */
            GoTo("Students/Update/" + studentId);
            updateEntryDateOfBirth = driver.FindElement(By.Id("CU_Student_DateOfBirth"));
            Assert.Equal(StudentDateOfBirthInput, updateEntryDateOfBirth.GetDomProperty("value"));

            DataClear(email);
        }

        protected override void CleanUp()
        {
            if (studentEmail != null)
            {
                DeleteStudent(studentEmail);
            }
        }

        // sisestab unikaalse e-postiga testõpilase ja tagastab selle e-posti, Dispose puhastab õpilase ka siis, kui test kukub
        private string DataEntry()
        {
            string email = UniqueValue("automaattestija") + "@selenium.ee";
            studentEmail = email;
            CreateStudent(email);
            return email;
        }

        // kustutab testõpilase, et järgmine test saaks puhtalt alustada
        private void DataClear(string email)
        {
            DeleteStudent(email);
            if (studentEmail == email)
            {
                studentEmail = null;
            }
        }

        private void CreateStudent(string email)
        {
            GoTo("");
            IWebElement navigateToStudents = driver.FindElement(By.LinkText("Õpilased")); //otsime testitava osa lingi
            navigateToStudents.Click();

            IWebElement indexCreateButton = driver.FindElement(By.Id("IndexCreateNewStudent_BTN"));
            indexCreateButton.Click();
            IWebElement createEntryFirstName = driver.FindElement(By.Id("CU_Student_FirstName"));
            createEntryFirstName.SendKeys(StudentFirstName);
            IWebElement createEntryLastName = driver.FindElement(By.Id("CU_Student_LastName"));
            createEntryLastName.SendKeys(StudentLastName);
            IWebElement createEntryEmail = driver.FindElement(By.Id("CU_Student_Email"));
            createEntryEmail.SendKeys(email);
            IWebElement createEntryDateOfBirth = driver.FindElement(By.Id("CU_Student_DateOfBirth"));
            //kuupäevavälja klahvidega täitmine sõltub brauseri keelest, seega paneme väärtuse otse
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].value = arguments[1];", createEntryDateOfBirth, StudentDateOfBirthInput);
            IWebElement createSaveButton = driver.FindElement(By.Id("CU_Student_Create_BTN"));
            createSaveButton.Click();
        }

        private void DeleteStudent(string email)
        {
            GoTo("Students");
            IWebElement? row = FindStudentRow(email);
            if (row == null)
            {
                return; //õpilast pole salvestatud, midagi muud me ei kustuta
            }

            IWebElement indexDeleteButton = row.FindElement(By.Id("IndexTableStudentDelete_BTN"));
            indexDeleteButton.Click();
            IWebElement deleteConfirmButton = driver.FindElement(By.Id("DetailsDelete_Delete_BTN"));
            deleteConfirmButton.Click();
        }

        // otsime indeksi tabelist rea, mille e-post klapib, tagastab null kui rida pole
        private IWebElement? FindStudentRow(string email)
        {
            IWebElement tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0); //päiserida

            foreach (var row in rowsOfTable)
            {
                var emailElement = row.FindElement(By.Id("IndexTableStudentEmail")).Text;
                if (emailElement == email)
                {
                    return row;
                }
            }

            return null;
        }
    }
}
