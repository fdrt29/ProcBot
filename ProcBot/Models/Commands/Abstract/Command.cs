using Telegram.Bot.Examples.WebHook.Models.Abstract;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.WebHook.Models.Commands.Abstract;

public abstract class Command : IHandler
{
    public abstract string? Name { get; }
    public static string? NameStatic { get; set; }

    public abstract Task Execute(Update update, BotContext db, User user);

    public virtual bool IsMatch(Update update, User user)
    {
        return EqualToName(update);
    }

    protected bool EqualToName(Update update)
    {
        return update.GetDataUniversal() == Name;
    }
}