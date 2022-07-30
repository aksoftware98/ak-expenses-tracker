using AKExpensesTracker.Shared.Enums;

namespace AKExpensesTracker.Shared.DTOs;

public class WalletSummaryDto
{
    public string? Id { get; set; }

    public WalletType Type { get; set; }

    public decimal Balacne { get; set; }
    public string? Name { get; set; }
    public string? Currency { get; set; }
}
