
using LocationTracker.Services;

namespace LocationTracker;

public sealed class App : Application
{
    private readonly LocationDatabase _database;

    public App(LocationDatabase database)
    {
        _database = database;
    }

    protected override Window CreateWindow(
        IActivationState? activationState)
    {
        return new Window(new MainPage(_database));
    }
}
