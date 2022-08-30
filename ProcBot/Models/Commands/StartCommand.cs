using Telegram.Bot.Examples.WebHook.Models.Commands.Abstract;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.WebHook.Models.Commands;

public class StartCommand : Command
{
    public override string? Name => NameStatic;
    public new static string NameStatic => @"/start";

    public override bool IsMatch(Update update, User user)
    {
        return EqualToName(update);
    }

    public override async Task Execute(Update update, BotContext db, User user)
    {
        user.State = UserState.Unidentified;
        await db.SaveChangesAsync();
    }
}