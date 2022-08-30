using System.Text;
using Telegram.Bot.Examples.WebHook.Models.Commands.Abstract;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.WebHook.Models.Commands;

public class LastOperationsCommand : CommandWithResult<string>
{
    public override string? Name => NameStatic;
    public new static string NameStatic => @"/lastOperations";

    public override string? Result { get; protected internal set; }

    public override async Task Execute(Update update, BotContext db, User user)
    {
        StringBuilder message = new();

        int amount = 10;
        IQueryable<Account> query = db.Accounts.Where(account => account.CompanyId == user.CompanyId)
            .OrderByDescending(account => account.AccountId)
            .Take(amount);

        foreach (Account account in query)
        {
            message.Append(account);
            message.AppendLine();
        }

        Result = message.ToString();
    }
}