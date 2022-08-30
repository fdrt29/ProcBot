namespace Telegram.Bot.Examples.WebHook.Models.Commands.Abstract;

public abstract class CommandWithResult<T> : Command
{
    public abstract T? Result { get; protected internal set; }
}