namespace Gasta.Shared.Abstractions;

/// <summary>
/// Lifecycle operations on the underlying database that are not part of any single
/// repository: open it once at startup, and wipe every table (restore / reset).
/// </summary>
public interface IAppDatabase
{
    Task InitializeAsync();
    Task ClearAllAsync();
}
