using System.Text.Json.Serialization;

namespace AKExpensesTracker.Server.Data.Models;

public class Wallet
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("type")]
    public string? TypeName { get; set; }

    public WalletType? Type => GetWalletTypeFromString(TypeName);

    [JsonPropertyName("bankName")]
    public string? BankName { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("iban")]
    public string? Iban { get; set; }

    [JsonPropertyName("accountType")]
    public string? AccountType { get; set; }

    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    [JsonPropertyName("swift")]
    public string? Swift { get; set; }

    [JsonPropertyName("balance")]
    public decimal Balance { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }


    private WalletType? GetWalletTypeFromString(string? typeName)
    {
        return typeName switch
        {
            "Bank" => WalletType.Bank,
            "PayPal" => WalletType.PayPal,
            "Cash" => WalletType.Cash,
            _ => WalletType.Other
        };
    }

}
