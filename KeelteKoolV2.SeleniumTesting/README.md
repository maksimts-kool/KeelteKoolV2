# Selenium testing

Browser smoke tests for KeelteKoolV2 using .NET 10, xUnit, and Selenium.
Install Mozilla Firefox. Selenium Manager obtains the matching GeckoDriver
automatically; the first run requires network access.

From the repository root, start the application in one terminal:

```sh
dotnet run --project KeelteKoolV2/KeelteKoolV2.csproj --launch-profile http
```

After the application is listening, run the tests in another terminal:

```sh
dotnet test KeelteKoolV2.SeleniumTesting/KeelteKoolV2.SeleniumTesting.csproj
```

`LanguageCourseFrontendTests` uses `http://localhost:5117/` and headless Firefox by
default. It has four tests (create, read, update, delete), each with a fresh browser. Each
test creates a language course through the form, checks the result in the UI and
then deletes the course again.

Set `SELENIUM_BASE_URL` to test a different running instance, and set
`SELENIUM_HEADLESS=false` to show the browser. For example, in Bash or Zsh:

```sh
SELENIUM_BASE_URL=http://localhost:5117/ SELENIUM_HEADLESS=false dotnet test KeelteKoolV2.SeleniumTesting/KeelteKoolV2.SeleniumTesting.csproj
```

Set `SELENIUM_SLOWMO` to a number of milliseconds (for example `700`) to pause
after every click, text entry and navigation so the browser is easier to follow:

```sh
SELENIUM_HEADLESS=false SELENIUM_SLOWMO=700 dotnet test KeelteKoolV2.SeleniumTesting/KeelteKoolV2.SeleniumTesting.csproj
```

In PowerShell, use `$env:SELENIUM_BASE_URL` and `$env:SELENIUM_HEADLESS` to set
these environment variables before running `dotnet test`.
