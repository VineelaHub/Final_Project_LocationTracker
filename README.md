# Location Tracker — Final Software Development Project

C# .NET MAUI Android/iOS app that collects GPS observations (only while open), persists them locally using SQLite, and visualizes density as translucent heat-map circles on the .NET MAUI Maps component.

## Prerequisites
- Visual Studio 2022 (current version) with .NET MAUI workload and .NET 9 SDK, or compatible setup. iOS builds require a Mac build host.
- Android emulator/device with Google Play services or an iOS simulator/device.
- Android only: a Google Maps SDK for Android API key (enable the API and restrict your key).

## Run
1. Open `LocationTracker/LocationTracker.csproj` in Visual Studio.
2. Restore NuGet dependencies. If your installed MAUI workload is a different SDK version, align TargetFrameworks and Maps package with that installed workload.
3. Edit `Platforms/Android/AndroidManifest.xml` and replace `YOUR_GOOGLE_MAPS_API_KEY` with your own key for Android.
4. Select an Android or iOS target and start debugging. Grant **While in use** location permission.
5. Tap **Save current** to store a point, or **Start tracking** for a GPS reading approximately every 15 seconds (only points at least 10 m apart are automatically saved). Move about with the app visible. Tap **Refresh heat map** to redraw. Tap **Clear history** to delete local records.
6. Capture an actual screenshot from the running app and insert it in the Word document, replacing the screenshot placeholder.
7. Upload `LocationTrackerProject` to a GitHub repository and replace `GITHUB_REPOSITORY_URL_HERE` in the Word document with your actual repository URL.

## Implementation
- `Models/LocationPoint.cs`: SQLite table model.
- `Services/LocationDatabase.cs`: async SQLite persistence.
- `Services/HeatMapRenderer.cs`: bins GPS points into approximately 100 m grid cells and draws layered translucent MAUI `Circle` overlays. Red = higher relative density.
- `MainPage.cs`: UI, foreground tracking, map refresh, permission handling, deletion confirmation.
- Platform manifests: Android GPS and Maps key, iOS location-usage explanation.

## Privacy and limits
- Local-only storage: no location data is transmitted by app code; map tile provider may make network requests.
- Foreground only: tracking stops when navigating away. Not a background tracking service.
- Native `Circle` overlays are a simplified density heat map rather than a smooth raster kernel-density surface.
- Android and iOS are targeted; Windows MAUI Maps does not support MapElement circle rendering.
- For larger datasets, add map-window filtering or tile-based aggregation; this example displays at most 180 density cells.
- This source was prepared for the assignment but could not be built or executed in this environment; validate on your emulator/device.

## References
- https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/map
- https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device/geolocation
- https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/appmodel/permissions
- https://github.com/praeclarum/sqlite-net
