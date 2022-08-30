using Telegram.Bot.Examples.WebHook.Models.UserInputHandlers.Abstract;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.WebHook.Models.Commands.CreateAccount;

public class EmailInputHandler : InputHandler<string>
{
    protected override UserState MatchingUserState => UserState.InputEmail;

    public override bool Success { get; set; }

    protected override bool TryGetParsedInput(string? userInput, out string parsedInput)
    {
        parsedInput = userInput ?? string.Empty;
        return true;
    }

    protected override async Task OnSuccessfulParsing(string email, Message message,
        BotContext db, User user)
    {
        await base.OnSuccessfulParsing(email, message, db, user);

        if (!db.TryGetDraftForUser(user, out AccountDraft? draft)) return;

        draft!.Email = email;

        if (user.CompanyId != null)
            await db.Accounts.AddAsync(new Account
            {
                Amount = user.AccountDraft.Amount,
                Description = user.AccountDraft.Description,
                Email = email,
                CompanyId = (int) user.CompanyId,
                DateTimeStamp = DateTime.Now
            });

        Company? company = await db.Companies.FindAsync(user.CompanyId);
        user.State = UserState.Identified;
        await db.SaveChangesAsync();

        await db.Entry(user)
            .Reference(b => b.Company)
            .LoadAsync();
    }

    protected override Task OnFailedParsing(Message message, BotContext db, User user)
    {
        return Task.CompletedTask;
    }
}