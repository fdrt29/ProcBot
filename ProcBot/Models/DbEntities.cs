using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Telegram.Bot.Examples.WebHook.Models;

public enum UserState
{
    Unidentified,
    InputId,
    Identified,
    CreateAccount,

    // Account creating states
    InputAmount,
    InputDescription,
    InputEmail
}

public class User
{
    [Key] public long UserId { get; set; }
    public int? CompanyId { get; set; }
    public UserState State { get; set; }

    [ForeignKey("CompanyId")] public Company? Company { get; set; }

    public AccountDraft? AccountDraft { get; set; }
}

public class AccountDraft
{
    [Key] public int AccountDraftId { get; set; }
    public string? Description { get; set; }
    public int Amount { get; set; }
    public string? Email { get; set; }

    public long UserId { get; set; }

    [ForeignKey("UserId")] public User User { get; set; }

    public override string ToString()
    {
        return $"Amount: {Amount}, Description: {Description}, Email: {Email}";
    }
}

public class Account
{
    [Key] public int AccountId { get; set; }
    public int CompanyId { get; set; }
    public string Description { get; set; }
    public int Amount { get; set; }
    public string Email { get; set; }
    public DateTime DateTimeStamp { get; set; }

    public override string ToString()
    {
        return $"Amount: {Amount}, Description: {Description}, Email: {Email}, Time: {DateTimeStamp}";
    }
}

public class Company
{
    [Key] public int CompanyId { get; set; }
    public string CompanyName { get; set; }

    public ICollection<User> Players { get; set; } = new List<User>();
    public ICollection<Account> Accounts { get; } = new List<Account>();
}