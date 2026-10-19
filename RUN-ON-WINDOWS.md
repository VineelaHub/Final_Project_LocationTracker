# Run the assignment on Windows

This is a C# .NET MAUI mobile app. Use the Android emulator on this computer; iOS requires a Mac.

## What was checked

- Visual Studio 2022 Community and the Android SDK are installed.
- The Pixel_8_Pro emulator is available and was connected during inspection.
- No .NET SDK was found by dotnet or Visual Studio's build tool. The installed .NET runtimes alone cannot compile this project.
- The existing signed APK installs but exits because it was built for Fast Deployment without the application assemblies. Rebuild it; the source now enables embedded assemblies.
- The source's database registration now supplies a file path in the app's private data directory. Previously its required string constructor argument was not registered.
- The Android manifest still contains YOUR_GOOGLE_MAPS_API_KEY.
- The edited source has not been compiled or run because the development SDK is missing.

## Set up and run

1. Open **Visual Studio Installer**, find **Visual Studio 2022 Community**, and select **Modify**.
2. Select **.NET Multi-platform App UI development**. Ensure the .NET 9 SDK and compatible MAUI 9 Android tooling are installed for this project's `net9.0-android` target. This project currently uses MAUI Controls 9.0.120. If your installer only offers a newer MAUI version, the project and packages must be migrated together before building.
3. Restart Visual Studio after setup. Open `LocationTracker/LocationTracker.csproj`.
4. In your Google Cloud project, enable **Maps SDK for Android** and create an API key. Restrict it to Android package `edu.cumberlands.locationtracker` and your actual signing certificate's SHA-1. Follow the provider's setup, including billing requirements if prompted. Do not publish an unrestricted key.
5. Replace `YOUR_GOOGLE_MAPS_API_KEY` in `LocationTracker/Platforms/Android/AndroidManifest.xml` with that key.
6. Select **Android** and **Pixel_8_Pro** in Visual Studio's run target menu. Restore NuGet packages, then choose **Build > Rebuild Solution** and press **F5**.
7. If Visual Studio reports old paths from another computer, close it, remove only this project's generated `LocationTracker/bin` and `LocationTracker/obj` folders, reopen, and restore/rebuild. These folders are build output, not source.

After the SDK and workloads are available, the equivalent PowerShell commands from the project root are:

```powershell
dotnet restore .\LocationTracker\LocationTracker.csproj -p:TargetFrameworks=net9.0-android
dotnet build .\LocationTracker\LocationTracker.csproj -f net9.0-android -p:TargetFrameworks=net9.0-android -t:Run
```

## Demonstrate the assignment

1. In the emulator, open **Extended controls (...) > Location**, choose a location, and send it to the device. Ensure Android Location is enabled.
2. In the app, tap **Save current**, granting location permission while using the app. Check that the saved count increases and the map centers on your point.
3. Tap **Start tracking**. Change the emulator location by more than 10 metres and wait at least 15 seconds. The saved count should increase. Use a route in the emulator to demonstrate movement.
4. Tap **Stop tracking**, then **Refresh heat map**. Nearby saved points appear as translucent circles, with denser areas redder. Manual saves can add repeated points to demonstrate density.
5. Close and reopen the app to confirm SQLite preserved the count.
6. Capture a real screenshot of the running app for the assignment. If required by your rubric, add the screenshot and your repository URL to your report; no report file was found in this folder.
7. Test **Clear history** last, because confirming it permanently removes the saved points.

The current implementation checks for location approximately every 15 seconds and saves automatic points only after movement of at least 10 metres. Its heat map groups points into approximate grid cells. Tracking is tied to page visibility; background/pause behavior still needs device testing.

## Official setup references

- https://learn.microsoft.com/dotnet/maui/get-started/installation
- https://learn.microsoft.com/dotnet/maui/user-interface/controls/map
