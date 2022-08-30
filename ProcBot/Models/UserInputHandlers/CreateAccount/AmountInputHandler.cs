using Telegram.Bot.Examples.WebHook.Models.UserInputHandlers.Abstract;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.WebHook.Models.Commands.CreateAccount;

public class AmountInputHandler : InputHandler<int>
{
    protected override UserState MatchingUserState => UserState.InputAmount;

    public override bool Success { get; set; }

    protected override bool TryGetParsedInput(string? userInput, out int parsedInput)
    {
        return int.TryParse(userInput, out parsedInput) && parsedInput > 0;
    }

    protected override async Task OnSuccessfulParsing(int amount, Message message,
        BotContext db,
        User user)
    {
        await base.OnSuccessfulParsing(amount, message, db, user);
        if (!db.TryGetDraftForUser(user, out AccountDraft? draft)) return;
        draft!.Amount = amount;

        user.State = UserState.InputDescription;
        await db.SaveChangesAsync();
    }
}