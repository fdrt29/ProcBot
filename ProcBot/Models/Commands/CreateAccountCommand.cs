using Telegram.Bot.Examples.WebHook.Models.Commands.Abstract;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.WebHook.Models.Commands;

public class CreateAccountCommand : Command
{
    public override string? Name => NameStatic;
    public new static string NameStatic => @"/createAccount";

    public override async Task Execute(Update update, BotContext db, User user)
    {
        if (db.TryGetDraftForUser(user, out AccountDraft? draft))
        {
            draft.Amount = 0;
            draft.Description = "";
            draft.Email = "";
        }
        else
        {
            draft = new AccountDraft {User = user, Amount = 0, Description = "", Email = ""};
            await db.AccountDrafts.AddAsync(draft);
        }

        user.State = UserState.InputAmount;

        await db.SaveChangesAsync();
    }

    public override bool IsMatch(Update update, User user)
    {
        return user is {State: UserState.Identified} && base.IsMatch(update, user);
    }
}