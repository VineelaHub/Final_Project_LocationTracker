using Map = Microsoft.Maui.Controls.Maps.Map;
using LocationTracker.Models;
using LocationTracker.Services;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;

namespace LocationTracker;

/// <summary>Foreground location collection: runs only while the page is visible.</summary>
public sealed class MainPage : ContentPage
{
    private readonly LocationDatabase _database;
    private readonly Map _map = new() { MapType = MapType.Street, IsShowingUser = false };
    private readonly Label _status = new() { Text = "Ready to track", FontSize = 13, TextColor = Colors.White };
    private readonly Label _count = new() { Text = "0 saved points", FontSize = 14, TextColor = Colors.White };
    private readonly Button _trackButton = new() { Text = "Start tracking", BackgroundColor = Color.FromArgb("#15A78E"), TextColor = Colors.White };
    private CancellationTokenSource? _trackingCancellation;
    private LocationPoint? _lastSaved;
    private bool _busy;
    private Task? _trackingTask;
    private bool _resetting;

    public MainPage(LocationDatabase database)
    {
        _database = database;
        Title = "MainView";
        BackgroundColor = Color.FromArgb("#3600B5");
        var header = new Grid { HeightRequest = 48, Padding = new Thickness(16, 0), BackgroundColor = Color.FromArgb("#3600B5") };
        header.Add(new Label { Text = "MainView", FontSize = 20, FontAttributes = FontAttributes.Bold, TextColor = Colors.White, VerticalOptions = LayoutOptions.Center });
        var newRoute = new Button { Text = "New route", FontSize = 13, TextColor = Colors.White, BackgroundColor = Colors.Transparent, HorizontalOptions = LayoutOptions.End, Padding = new Thickness(10, 0) };
        newRoute.Clicked += async (_, _) => await ClearAsync();
        header.Add(newRoute);
        var layout = new Grid { RowDefinitions = new RowDefinitionCollection { new(GridLength.Auto), new(GridLength.Star) } };
        layout.Add(header, 0, 0);
        layout.Add(_map, 0, 1);
        Content = layout;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshMapAsync();
        if (_trackingCancellation is null) await ToggleTrackingAsync();
    }

    protected override void OnDisappearing()
    {
        StopTracking();
        base.OnDisappearing();
    }

    private async Task<bool> EnsurePermissionAsync()
    {
        var permission = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (permission != PermissionStatus.Granted)
            permission = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (permission == PermissionStatus.Granted) return true;
        _status.Text = "Location permission denied";
        await DisplayAlert("Permission required", "Enable location permission in device Settings to collect GPS points.", "OK");
        return false;
    }

    private async Task ToggleTrackingAsync()
    {
        if (_trackingCancellation is not null) { StopTracking(); return; }
        if (!await EnsurePermissionAsync()) return;
        _map.IsShowingUser = true;
        var cts = new CancellationTokenSource();
        _trackingCancellation = cts;
        _trackButton.Text = "Stop tracking";
        _status.Text = "Tracking (foreground)";
        _trackingTask = TrackingLoopAsync(cts.Token);
    }

    private void StopTracking()
    {
        var cts = _trackingCancellation;
        _trackingCancellation = null;
        cts?.Cancel();
        _trackButton.Text = "Start tracking";
        _status.Text = "Tracking stopped";
    }

    private async Task TrackingLoopAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await CaptureAsync(force: false, token);
                await Task.Delay(TimeSpan.FromSeconds(15), token);
            }
        }
        catch (OperationCanceledException) { /* Expected on Stop or leaving the page. */ }
        finally
        {
            if (!token.IsCancellationRequested)
                MainThread.BeginInvokeOnMainThread(StopTracking);
        }
    }

    private async Task CaptureAsync(bool force, CancellationToken token = default)
    {
        if (_busy) return;
        _busy = true;
        try
        {
            if (!await EnsurePermissionAsync()) return;
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var current = await Geolocation.Default.GetLocationAsync(request, token);
            if (current is null) { _status.Text = "No GPS fix available"; return; }
            if (!force && _lastSaved is not null)
            {
                var previous = new Location(_lastSaved.Latitude, _lastSaved.Longitude);
                if (Location.CalculateDistance(previous, current, DistanceUnits.Kilometers) < 0.01)
                {
                    _status.Text = "Tracking: waiting for movement";
                    return;
                }
            }
            var point = new LocationPoint
            {
                Latitude = current.Latitude,
                Longitude = current.Longitude,
                AccuracyMeters = current.Accuracy,
                TimestampUtc = DateTime.UtcNow
            };
            await _database.SaveAsync(point);
            _lastSaved = point;
            _status.Text = "Location saved";
            await RefreshMapAsync();
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) when (ex is PermissionException or FeatureNotEnabledException or FeatureNotSupportedException)
        {
            _status.Text = "Location unavailable";
            await DisplayAlert("Location error", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            _status.Text = "Unable to save location";
            await DisplayAlert("Error", ex.Message, "OK");
        }
        finally { _busy = false; }
    }

    private async Task RefreshMapAsync()
    {
        var points = await _database.GetPointsAsync();
        _count.Text = $"{points.Count} saved points";
        HeatMapRenderer.Render(_map, points);
        if (points.Count > 0)
        {
            _lastSaved = points[^1];
            _map.MoveToRegion(MapSpan.FromCenterAndRadius(
                new Location(_lastSaved.Latitude, _lastSaved.Longitude), Distance.FromKilometers(1.3)));
        }
    }

    private async Task ClearAsync()
    {
        if (_resetting) return;
        _resetting = true;
        try
        {
            bool confirm = await DisplayAlert("Start a new route?", "Delete saved location history and begin recording from your current position?", "New route", "Cancel");
            if (!confirm) return;
            StopTracking();
            // Finish any in-flight save before clearing, so old points cannot reappear.
            if (_trackingTask is not null) await _trackingTask;
            await _database.ClearAsync();
            _lastSaved = null;
            await RefreshMapAsync();
            await ToggleTrackingAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Unable to start route", ex.Message, "OK");
        }
        finally { _resetting = false; }
    }
}

