using LocationTracker.Models;
using SQLite;

namespace LocationTracker.Services;

/// <summary>Asynchronous SQLite access, with one-time table initialization.</summary>
public sealed class LocationDatabase
{
    private readonly SQLiteAsyncConnection _connection;
    private readonly SemaphoreSlim _initializeLock = new(1, 1);
    private bool _initialized;

    public LocationDatabase(string path)
    {
        _connection = new SQLiteAsyncConnection(path);
    }

    private async Task InitializeAsync()
    {
        if (_initialized) return;
        await _initializeLock.WaitAsync();
        try
        {
            if (_initialized) return;
            await _connection.CreateTableAsync<LocationPoint>();
            _initialized = true;
        }
        finally { _initializeLock.Release(); }
    }

    public async Task<List<LocationPoint>> GetPointsAsync()
    {
        await InitializeAsync();
        return await _connection.Table<LocationPoint>()
            .OrderBy(point => point.TimestampUtc).ToListAsync();
    }

    public async Task SaveAsync(LocationPoint point)
    {
        await InitializeAsync();
        await _connection.InsertAsync(point);
    }

    public async Task ClearAsync()
    {
        await InitializeAsync();
        await _connection.DeleteAllAsync<LocationPoint>();
    }
}
