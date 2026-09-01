namespace Gasta.Helpers;

/// <summary>
/// Derives lightened/darkened tones from a hex color — the Blazor equivalent of the
/// MAUI build's ColorKeyConverter "Bg" parameter. Same blend-toward-white/black math,
/// just called directly from markup instead of through an XAML converter, since Blazor
/// has no converter concept.
///
/// Usage: style="stroke:@ColorHelper.Lighten(category.ColorKey)"
/// </summary>
public static class ColorHelper
{
    // Same default as the MAUI build — reverse-engineered from a concrete Figma example
    // (#FFCD9D -> #FFE9D4), not from an "X% opacity" guess.
    private const double DefaultBlendAmount = 0.56;

    public static string Lighten(string hex, double amount = DefaultBlendAmount) =>
        Blend(hex, amount, towardWhite: true);

    public static string Darken(string hex, double amount = 0.3) =>
        Blend(hex, amount, towardWhite: false);

    private static string Blend(string hex, double amount, bool towardWhite)
    {
        if (string.IsNullOrWhiteSpace(hex)) return "#9AA0A6";

        hex = hex.TrimStart('#');
        if (hex.Length != 6 ||
            !int.TryParse(hex[..2], System.Globalization.NumberStyles.HexNumber, null, out var r) ||
            !int.TryParse(hex[2..4], System.Globalization.NumberStyles.HexNumber, null, out var g) ||
            !int.TryParse(hex[4..6], System.Globalization.NumberStyles.HexNumber, null, out var b))
        {
            return "#" + hex;
        }

        var target = towardWhite ? 255 : 0;
        r = Clamp((int)(r + (target - r) * amount));
        g = Clamp((int)(g + (target - g) * amount));
        b = Clamp((int)(b + (target - b) * amount));

        return $"#{r:X2}{g:X2}{b:X2}";
    }

    private static int Clamp(int v) => Math.Clamp(v, 0, 255);
}