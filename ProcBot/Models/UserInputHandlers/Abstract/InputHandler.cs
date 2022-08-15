using Telegram.Bot.Examples.WebHook.Models.Abstract;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.WebHook.Models.UserInputHandlers.Abstract;

public abstract class InputHandler : IHandler
{
    public abstract bool Success { get; set; }
    public abstract Task Execute(Update update, BotContext db, User user);

    public abstract bool IsMatch(Update update, User user);
}

public abstract class InputHandler<T> : InputHandler
{
    protected abstract UserState MatchingUserState { get; }

    protected abstract bool TryGetParsedInput(string? userInput, out T parsedInput);

    protected virtual Task OnSuccessfulParsing(T parsedInput, Message message, BotContext db,
        User user)
    {
        Success = true;
        return Task.CompletedTask;
    }

    protected virtual Task OnFailedParsing(Message message, BotContext db, User user)
    {
        Success = false;
        return Task.CompletedTask;
    }

    public override bool IsMatch(Update update, User user)
    {
        return user?.State == MatchingUserState;
    }

    public override async Task Execute(Update update, BotContext db, User user)
    {
        Message message = update.GetMessageUniversal();
        if (TryGetParsedInput(message.Text, out T parsedInput))
            await OnSuccessfulParsing(parsedInput, message, db, user);
        else
            await OnFailedParsing(message, db, user);
    }
}