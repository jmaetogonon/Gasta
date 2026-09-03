using Gasta.Data.Repositories;
using Gasta.Models;

namespace Gasta.Services;

public class SeedService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IBudgetRepository _budgetRepository;

    public SeedService(
        ICategoryRepository categoryRepository,
        IPaymentMethodRepository paymentMethodRepository,
        IBudgetRepository budgetRepository)
    {
        _categoryRepository = categoryRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _budgetRepository = budgetRepository;
    }

    public async Task SeedIfEmptyAsync()
    {
        if ((await _categoryRepository.GetActiveOrderedAsync()).Count > 0)
            return;

        var categories = new List<Category>
        {
            // All of these were originally in the 70-84% lightness range — pastel enough
            // that white text (used directly on top of these colors in AddExpenseSheet's
            // header banner and Save button) had genuinely poor contrast. Deepened into a
            // consistent ~55-68% lightness band: still clearly reads as each category's
            // color, pops against the app's pale lavender surfaces, and gives white text
            // enough contrast to read comfortably.
            new() { Name = "Shoppee", LogoImage = "shopeecat", ColorKey = "#FF7A52", MonthlyBudget = 5000, SortOrder = 0 },
            new() { Name = "Lazada", LogoImage = "lazadacat", ColorKey = "#FF4FA3", MonthlyBudget = 5000, SortOrder = 1 },
            new() { Name = "Grab", LogoImage = "grab", ColorKey = "#4ECD82", MonthlyBudget = 5000, SortOrder = 2 },
            new() { Name = "Eat Out", LogoImage = "eatout", ColorKey = "#FFA94D", MonthlyBudget = 5000, SortOrder = 3 },
            new() { Name = "Groceries", LogoImage = "groceries", ColorKey = "#E8B93E", MonthlyBudget = 5000, SortOrder = 4 },
            new() { Name = "Health", LogoImage = "health", ColorKey = "#FF8FAE", MonthlyBudget = 5000, SortOrder = 5 },
            new() { Name = "Transpo", LogoImage = "transpo", ColorKey = "#45C7D1", MonthlyBudget = 5000, SortOrder = 6 },
            new() { Name = "Gifts", LogoImage = "gifts", ColorKey = "#A968EE", MonthlyBudget = 5000, SortOrder = 7 },
        };
        foreach (var c in categories)
            await _categoryRepository.SaveAsync(c);

        var paymentMethods = new List<PaymentMethod>
        {
            new() { Name = "GCash", LogoImage = "gcash", ColorKey = "#d9daff", SortOrder = 0 },
            new() { Name = "Credit Card", LogoImage = "creditcard", ColorKey = "#fed6d7", SortOrder = 1 },
            new() { Name = "Cash", LogoImage = "cash", ColorKey = "#ceffcf", SortOrder = 2 },
            new() { Name = "Maya", LogoImage = "maya", ColorKey = "#1F1B18", SortOrder = 3 },
            new() { Name = "Debit Card", LogoImage = "debitcard", ColorKey = "#ffe0c3", SortOrder = 4 },
            new() { Name = "MariBank", LogoImage = "maribank", ColorKey = "#f8e9df", SortOrder = 5 },
            new() { Name = "GOtyme bank", LogoImage = "gotyme", ColorKey = "#a3eef3", SortOrder = 6 },
            new() { Name = "Shoppee Pay", LogoImage = "shopeepay", ColorKey = "#ffd3cc", SortOrder = 7 },
        };
        foreach (var m in paymentMethods)
            await _paymentMethodRepository.SaveAsync(m);

        var now = DateTime.Now;
        await _budgetRepository.SetForMonthAsync(now.Year, now.Month, 5000.0m);
    }
}