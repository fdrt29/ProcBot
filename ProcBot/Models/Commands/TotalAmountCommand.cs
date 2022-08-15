using Telegram.Bot.Examples.WebHook.Models.Commands.Abstract;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.WebHook.Models.Commands;

public class TotalAmountCommand : CommandWithResult<int>
{
    public override string? Name => NameStatic;
    public new static string NameStatic => "/amount";

    public override int Result { get; protected internal set; }

    public override async Task Execute(Update update, BotContext db, User user)
    {
        DateTime startDateTime = DateTime.Today; //Today at 00:00:00
        DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59

        IQueryable<int> amounts = db.Accounts.Where(account => account.CompanyId == user.CompanyId)
            .OrderByDescending(n => n.DateTimeStamp)
            .Where(a => a.DateTimeStamp >= startDateTime && a.DateTimeStamp <= endDateTime)
            .Select(a => a.Amount);

        Result = amounts.Sum();
    }
}