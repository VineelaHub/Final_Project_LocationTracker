# Location Tracker

C# .NET MAUI assignment app using native Maps circle overlays, foreground GPS, and SQLite.

## Interface and behavior
Purple MainView title bar, full-page map, automatic foreground tracking, and New route confirmation. Captures approximately every 15 seconds after each request completes; automatic saves require at least 10 metres of movement. Red/orange layered circle overlays approximate location density. New route deletes saved history and restarts collection.

## Android setup
Install .NET 9 SDK, compatible MAUI Android tooling, a JDK, and Android SDK platform 35. Use an emulator with Google Play services. Replace YOUR_GOOGLE_MAPS_API_KEY in LocationTracker/Platforms/Android/AndroidManifest.xml with a restricted Maps SDK for Android key. Package: edu.cumberlands.locationtracker. Configure your own signing certificate SHA-1 in Google Cloud.

From the LocationTracker folder in PowerShell:

```powershell
dotnet restore .\LocationTracker.csproj -p:TargetFrameworks=net9.0-android
dotnet build .\LocationTracker.csproj -f net9.0-android -p:TargetFrameworks=net9.0-android "-p:AndroidSdkDirectory=$env:LOCALAPPDATA\Android\Sdk" -t:Run
```

Grant location permission while using the app. In emulator Location controls, select a start, tap New route in the app, confirm, and play a route. A different emulator route alone does not clear stored app history.

## Source organization
- MainPage.cs: map layout, permission requests, capture loop, and route reset.
- Models/LocationPoint.cs: SQLite observation model.
- Services/LocationDatabase.cs: asynchronous persistence.
- Services/HeatMapRenderer.cs: layered circle heat map.
- MauiProgram.cs: Maps setup and database dependency injection.
- Platforms/Android/AndroidManifest.xml: permissions and API key placeholder.
- Platforms/iOS: iOS entry points and location usage description.

## Validation and limits
Android build and emulator screenshots verified map display and a recorded trail. The final reset change passed compilation; verify reset and persistence in the final demo. iOS is targeted but untested and requires a Mac. Tracking is not a background service. SQLite's existing native dependency produced 16 KB page-size warnings; newer-device compatibility needs separate validation. Heat rendering uses geographic circles, not a continuous raster density algorithm.

Never commit signing keystores or unrestricted API keys. The submission copy intentionally contains a key placeholder.
