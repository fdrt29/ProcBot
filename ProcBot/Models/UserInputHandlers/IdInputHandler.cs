using Telegram.Bot.Examples.WebHook.Models.UserInputHandlers.Abstract;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.WebHook.Models.UserInputHandlers;

public class IdInputHandler : InputHandler<int>
{
    protected override UserState MatchingUserState => UserState.InputId;

    public override bool Success { get; set; }

    protected override bool TryGetParsedInput(string? userInput, out int parsedInput)
    {
        return int.TryParse(userInput, out parsedInput);
    }

    protected override async Task OnSuccessfulParsing(int id, Message message,
        BotContext db, User user)
    {
        await base.OnSuccessfulParsing(id, message, db, user);
        if (db.TryGetCompanyById(id, out Company? company))
        {
            user.Company = company!;
            user.State = UserState.Identified;
            await db.SaveChangesAsync();

            Success = true;
        }
        else
        {
            await OnFailedParsing(message, db, user);
        }
    }
}