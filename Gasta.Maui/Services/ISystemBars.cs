namespace Gasta.Maui.Services;

/// <summary>
/// Lets the page tell the native shell what colour the app background is, so the strips
/// the system bars sit on (status bar on top, navigation bar at the bottom) match the
/// current theme and accent instead of showing a fixed colour.
/// </summary>
public interface ISystemBars
{
    void Apply(string hexColor, bool darkTheme);
}

/// <summary>Used on platforms that have no native implementation yet.</summary>
public class NoOpSystemBars : ISystemBars
{
    public void Apply(string hexColor, bool darkTheme) { }
}
