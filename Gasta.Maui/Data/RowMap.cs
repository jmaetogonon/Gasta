using System.Globalization;
using Gasta.Shared.Models;

namespace Gasta.Maui.Data;

internal static class RowMap
{
    private static string Money(decimal d) => d.ToString(CultureInfo.InvariantCulture);
    private static decimal Money(string s) => decimal.Parse(s, NumberStyles.Number, CultureInfo.InvariantCulture);

    public static ExpenseRow ToRow(Expense e) => new()
    {
        Id = e.Id, DateTicks = e.Date.Ticks, Amount = Money(e.Amount),
        CategoryId = e.CategoryId, PaymentMethodId = e.PaymentMethodId, Notes = e.Notes
    };
    public static Expense ToModel(ExpenseRow r) => new()
    {
        Id = r.Id, Date = new DateTime(r.DateTicks), Amount = Money(r.Amount),
        CategoryId = r.CategoryId, PaymentMethodId = r.PaymentMethodId, Notes = r.Notes
    };

    public static CategoryRow ToRow(Category c) => new()
    {
        Id = c.Id, Name = c.Name, LogoImage = c.LogoImage, IconKey = c.IconKey,
        ColorKey = c.ColorKey, SortOrder = c.SortOrder
    };
    public static Category ToModel(CategoryRow r) => new()
    {
        Id = r.Id, Name = r.Name, LogoImage = r.LogoImage, IconKey = r.IconKey,
        ColorKey = r.ColorKey, SortOrder = r.SortOrder
    };

    public static PaymentMethodRow ToRow(PaymentMethod m) => new()
    {
        Id = m.Id, Name = m.Name, LogoImage = m.LogoImage, ColorKey = m.ColorKey, SortOrder = m.SortOrder
    };
    public static PaymentMethod ToModel(PaymentMethodRow r) => new()
    {
        Id = r.Id, Name = r.Name, LogoImage = r.LogoImage, ColorKey = r.ColorKey, SortOrder = r.SortOrder
    };

    public static BudgetRow ToRow(Budget b) => new() { Id = b.Id, Year = b.Year, Month = b.Month, Amount = Money(b.Amount) };
    public static Budget ToModel(BudgetRow r) => new() { Id = r.Id, Year = r.Year, Month = r.Month, Amount = Money(r.Amount) };

    public static CategoryBudgetRow ToRow(CategoryBudget b) => new()
    {
        Id = b.Id, CategoryId = b.CategoryId, Year = b.Year, Month = b.Month, Amount = Money(b.Amount)
    };
    public static CategoryBudget ToModel(CategoryBudgetRow r) => new()
    {
        Id = r.Id, CategoryId = r.CategoryId, Year = r.Year, Month = r.Month, Amount = Money(r.Amount)
    };
}
