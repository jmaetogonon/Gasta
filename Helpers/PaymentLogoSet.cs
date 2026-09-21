namespace Gasta.Helpers;

/// <summary>
/// Bundled payment-method logo filenames under wwwroot/images/payments/ (no extension).
/// Mirrors CategoryIconSet's role as "the known set a picker offers" — but these are
/// real brand image assets, not hand-authored SVG paths, so this is just a name list,
/// not inline markup. Anything not in this list falls back to an initial-letter badge
/// (see PaymentMethodIcon) rather than failing to render.
/// </summary>
public static class PaymentLogoSet
{
    public static readonly string[] Logos =
    [
        "gcash", "creditcard", "cash", "maya", "debitcard", "maribank", "gotyme", "shopeepay", "bdo"
    ];
}