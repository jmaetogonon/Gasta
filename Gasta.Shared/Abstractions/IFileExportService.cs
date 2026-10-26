namespace Gasta.Shared.Abstractions;

/// <summary>
/// Hands a generated text file (JSON backup, CSV export) to the user. On the web that is
/// a browser download; in the MAUI app it will be the Android share / save sheet.
/// </summary>
public interface IFileExportService
{
    Task SaveAsync(string fileName, string mimeType, string content);
}
