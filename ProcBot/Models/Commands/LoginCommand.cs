using Telegram.Bot.Examples.WebHook.Models.Commands.Abstract;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.WebHook.Models.Commands;

public class LoginCommand : Command
{
    public override string? Name => NameStatic;
    public new static string NameStatic => @"/login";

    public override async Task Execute(Update update, BotContext db, User user)
    {
        user.State = UserState.InputId;
        await db.SaveChangesAsync();
    }

    public override bool IsMatch(Update update, User user)
    {
        return user?.State == UserState.Unidentified && base.IsMatch(update, user);
    }
}