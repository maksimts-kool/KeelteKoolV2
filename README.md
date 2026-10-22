# KKV2
for testing

## Tests

- Unit tests: `dotnet test KeelteKoolV2.xUnitTesting/KeelteKoolV2.xUnitTesting.csproj`
- UI tests (Selenium + Firefox): start the app, then run the UI tests.

```sh
dotnet run --project KeelteKoolV2/KeelteKoolV2.csproj --launch-profile http
```

```sh
dotnet test KeelteKoolV2.SeleniumTesting/KeelteKoolV2.SeleniumTesting.csproj
```

See [KeelteKoolV2.SeleniumTesting/README.md](KeelteKoolV2.SeleniumTesting/README.md)
for database setup, settings and how to add new UI tests.
