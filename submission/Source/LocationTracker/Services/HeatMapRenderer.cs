using Map = Microsoft.Maui.Controls.Maps.Map;
using LocationTracker.Models;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;

namespace LocationTracker.Services;

/// <summary>
/// Approximates a heat map with layered native MAUI circles at saved GPS observations.
/// Overlapping translucent circles make frequently visited areas darker red.
/// </summary>
public static class HeatMapRenderer
{
    public static void Render(Map map, IReadOnlyList<LocationPoint> points)
    {
        map.MapElements.Clear();
        // Draw all halos first so they do not cover the darker centers of nearby points.
        var layers = new[]
        {
            (Radius: 65d, Color: Color.FromRgba(255, 170, 30, 40)),
            (Radius: 42d, Color: Color.FromRgba(255, 65, 20, 85)),
            (Radius: 25d, Color: Color.FromRgba(210, 0, 35, 145))
        };
        foreach (var layer in layers)
        {
            foreach (var point in points)
            {
                map.MapElements.Add(new Circle
                {
                    Center = new Location(point.Latitude, point.Longitude),
                    Radius = Distance.FromMeters(layer.Radius),
                    FillColor = layer.Color,
                    StrokeColor = Colors.Transparent,
                    StrokeWidth = 0
                });
            }
        }
    }
}
