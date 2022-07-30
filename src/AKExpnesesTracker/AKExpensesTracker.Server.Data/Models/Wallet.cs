using Newtonsoft.Json;

namespace AKExpensesTracker.Server.Data.Models;

public class Wallet
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("type")]
    public string? WalletType { get; set; }

    public WalletType? Type => GetWalletTypeFromString(WalletType);

    [JsonProperty("bankName")]
    public string? BankName { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("iban")]
    public string? Iban { get; set; }

    [JsonProperty("accountType")]
    public string? AccountType { get; set; }

    [JsonProperty("userId")]
    public string? UserId { get; set; }

    [JsonProperty("swift")]
    public string? Swift { get; set; }

    [JsonProperty("balance")]
    public decimal Balance { get; set; }

    [JsonProperty("currency")]
    public string? Currency { get; set; }

    [JsonProperty("username")]
    public string? Username { get; set; }

    [JsonProperty("creationDate")]
    public DateTime CreationDate { get; set; }

    [JsonProperty("modificationDate")]
    public DateTime ModificationDate { get; set; }

    private WalletType? GetWalletTypeFromString(string? typeName)
    {
        return typeName switch
        {
            "Bank" => Shared.Enums.WalletType.Bank,
            "PayPal" => Shared.Enums.WalletType.PayPal,
            "Cash" => Shared.Enums.WalletType.Cash,
            _ => Shared.Enums.WalletType.Other
        };
    }

}
