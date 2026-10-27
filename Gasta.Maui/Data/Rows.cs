using SQLite;

namespace Gasta.Maui.Data;

// Storage rows. The shared models stay free of database attributes, so each table has its own
// row class here plus a mapper (RowMap). Two deliberate choices:
//   * decimal amounts are stored as invariant-culture TEXT, never REAL, so money can't pick up
//     floating-point error and never depends on the phone's language/region.
//   * dates are stored as DateTime.Ticks (a plain integer), so they can't be reinterpreted by a
//     time zone or culture on the way back.
// Ids are AUTOINCREMENT, i.e. never reused after a delete or a reset (same as the IndexedDB
// auto-increment key the web version used).

public interface IRow
{
    int Id { get; set; }
}

[Table("expenses")]
public class ExpenseRow : IRow
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public long DateTicks { get; set; }
    public string Amount { get; set; } = "0";
    public int CategoryId { get; set; }
    public int PaymentMethodId { get; set; }
    public string? Notes { get; set; }
}

[Table("categories")]
public class CategoryRow : IRow
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public string Name { get; set; } = "";
    public string LogoImage { get; set; } = "";
    public string? IconKey { get; set; }
    public string ColorKey { get; set; } = "";
    public int SortOrder { get; set; }
}

[Table("paymentMethods")]
public class PaymentMethodRow : IRow
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public string Name { get; set; } = "";
    public string LogoImage { get; set; } = "";
    public string ColorKey { get; set; } = "";
    public int SortOrder { get; set; }
}

[Table("budgets")]
public class BudgetRow : IRow
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string Amount { get; set; } = "0";
}

[Table("categoryBudgets")]
public class CategoryBudgetRow : IRow
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public int CategoryId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string Amount { get; set; } = "0";
}
