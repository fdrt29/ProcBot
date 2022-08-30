using Telegram.Bot.Examples.WebHook.Models.UserInputHandlers.Abstract;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.WebHook.Models.Commands.CreateAccount;

public class DescriptionInputHandler : InputHandler<string>
{
    protected override UserState MatchingUserState => UserState.InputDescription;

    public override bool Success { get; set; }

    protected override bool TryGetParsedInput(string? userInput, out string parsedInput)
    {
        parsedInput = userInput ?? string.Empty;
        return true;
    }

    protected override async Task OnSuccessfulParsing(string description, Message message,
        BotContext db, User user)
    {
        if (!db.TryGetDraftForUser(user, out AccountDraft? draft)) return;

        draft!.Description = description;
        user.State = UserState.InputEmail;
        await db.SaveChangesAsync();
    }
}