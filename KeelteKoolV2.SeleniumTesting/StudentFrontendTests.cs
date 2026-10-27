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
