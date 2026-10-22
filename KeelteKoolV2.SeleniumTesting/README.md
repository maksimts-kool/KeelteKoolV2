# Selenium testing

Browser UI tests for KeelteKoolV2 using .NET 10, xUnit, and Selenium WebDriver.
Install Mozilla Firefox. Selenium Manager obtains the matching GeckoDriver
automatically; the first run requires network access.

## Database

The app uses a local SQLite file (`KeelteKoolV2/KeelteKoolV2.db`, not in Git).
On a fresh clone, or after pulling new migrations, create/update it from the
repository root:

```sh
dotnet ef database update --project KeelteKoolV2.Data --startup-project KeelteKoolV2
```

Tests do not need any existing rows: every test creates its own data with a
unique value (see `UniqueValue`) and deletes it again in `CleanUp`, even when
the test fails.

## Running

From the repository root, start the application in one terminal:

```sh
dotnet run --project KeelteKoolV2/KeelteKoolV2.csproj --launch-profile http
```

After the application is listening, run the tests in another terminal:

```sh
dotnet test KeelteKoolV2.SeleniumTesting/KeelteKoolV2.SeleniumTesting.csproj
```

Run only one module's tests with a filter, for example:

```sh
dotnet test KeelteKoolV2.SeleniumTesting/KeelteKoolV2.SeleniumTesting.csproj --filter FullyQualifiedName~LanguageCourseFrontendTests
```

## Settings

All settings are environment variables read by `SeleniumTestBase`:

| Variable | Default | Meaning |
| --- | --- | --- |
| `SELENIUM_BASE_URL` | `http://localhost:5117/` | Address of the running app |
| `SELENIUM_HEADLESS` | `false` | `true` hides the Firefox window |
| `SELENIUM_WAIT_SECONDS` | `10` | Implicit wait and explicit wait timeout |
| `SELENIUM_SLOWMO` | off | Milliseconds to pause after every click, text entry and navigation |

For example, in Bash or Zsh:

```sh
SELENIUM_HEADLESS=true dotnet test KeelteKoolV2.SeleniumTesting/KeelteKoolV2.SeleniumTesting.csproj
```

```sh
SELENIUM_SLOWMO=700 dotnet test KeelteKoolV2.SeleniumTesting/KeelteKoolV2.SeleniumTesting.csproj
```

In PowerShell, use `$env:SELENIUM_BASE_URL` etc. to set these before running
`dotnet test`.

## Writing a new test class

Inherit from `SeleniumTestBase`. Each test gets a fresh Firefox (xUnit creates a
new class instance per test). The base gives you:

- `driver` and `wait` (a `WebDriverWait` with the shared timeout)
- `GoTo("Students")` to open a page relative to the base URL
- `WaitUntilVisible(By)` and `WaitUntilUrlContains(string)` for explicit waits
- `UniqueValue(prefix)` for data your test can find again
- `CleanUp()` to override for deleting the test's own data

To run tests in a fixed order, put
`[TestCaseOrderer("KeelteKoolV2.SeleniumTesting.PriorityOrderer", "KeelteKoolV2.SeleniumTesting")]`
on the class and `[TestPriority(n)]` on each test (see `TestOrdering.cs`).
Add a short Estonian summary at the top of every test: what it does and what it
checks.

`LanguageCourseFrontendTests` is the reference example: four tests (create,
read, update, delete), each creating a language course through the form,
checking the result in the UI and deleting the course again.
