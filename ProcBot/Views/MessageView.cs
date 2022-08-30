using Telegram.Bot.Examples.WebHook.Models.Abstract;
using Telegram.Bot.Examples.WebHook.Models.Commands;
using Telegram.Bot.Examples.WebHook.Models.Commands.CreateAccount;
using Telegram.Bot.Examples.WebHook.Models.UserInputHandlers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = Telegram.Bot.Examples.WebHook.Models.User;

namespace Telegram.Bot.Examples.WebHook.Views;

public static class Keyboards
{
    public static readonly InlineKeyboardMarkup StartKeyboard = new[]
    {
        InlineKeyboardButton.WithCallbackData("Login", LoginCommand.NameStatic)
    };

    public static readonly InlineKeyboardMarkup Keyboard = new[]
    {
        InlineKeyboardButton.WithCallbackData("Create account", CreateAccountCommand.NameStatic),
        InlineKeyboardButton.WithCallbackData("Last 10 operations", LastOperationsCommand.NameStatic),
        InlineKeyboardButton.WithCallbackData("Amount of payments per day", TotalAmountCommand.NameStatic)
    };
}

public struct MessageView
{
    public string Text;
    public ParseMode? ParseMode;
    public IEnumerable<MessageEntity>? Entities;
    public bool? DisableWebPagePreview;
    public bool? DisableNotification;
    public bool? ProtectContent;
    public int? ReplyToMessageId;
    public bool? AllowSendingWithoutReply;
    public IReplyMarkup? ReplyMarkup;

    public MessageView(string text, ParseMode? parseMode = default, IEnumerable<MessageEntity>? entities = default,
        bool? disableWebPagePreview = default, bool? disableNotification = default, bool? protectContent = default,
        int? replyToMessageId = default, bool? allowSendingWithoutReply = default, IReplyMarkup? replyMarkup = default)
    {
        Text = text;
        ParseMode = parseMode;
        Entities = entities;
        DisableWebPagePreview = disableWebPagePreview;
        DisableNotification = disableNotification;
        ProtectContent = protectContent;
        ReplyToMessageId = replyToMessageId;
        AllowSendingWithoutReply = allowSendingWithoutReply;
        ReplyMarkup = replyMarkup;
    }


    public void Deconstruct(
        out string text, out ParseMode? parseMode, out IEnumerable<MessageEntity>? entities, out
            bool? disableWebPagePreview, out bool? disableNotification, out bool? protectContent, out
            int? replyToMessageId, out bool? allowSendingWithoutReply, out IReplyMarkup? replyMarkup)
    {
        text = Text;
        parseMode = ParseMode;
        entities = Entities;
        disableWebPagePreview = DisableWebPagePreview;
        disableNotification = DisableNotification;
        protectContent = ProtectContent;
        replyToMessageId = ReplyToMessageId;
        allowSendingWithoutReply = AllowSendingWithoutReply;
        replyMarkup = ReplyMarkup;
    }

    public static MessageView GetView(IHandler executedHandler, User user)
    {
        return executedHandler switch
        {
            StartCommand startCommand =>
                new MessageView("*Hallo* I'm ASP.NET Core Bot", Types.Enums.ParseMode.Markdown,
                    replyMarkup: Keyboards.StartKeyboard),

            CreateAccountCommand createAccountCommand =>
                new MessageView("Создание счета. Введите сумму:"),

            LastOperationsCommand lastOperationsCommand =>
                new MessageView("Последние 10 операций:\n" + lastOperationsCommand.Result + "\nВыберите действие:",
                    replyMarkup: Keyboards.Keyboard),

            LoginCommand loginCommand =>
                new MessageView("Введите ID компании:"),

            TotalAmountCommand totalAmountCommand =>
                new MessageView($"Общая сумма операций за день: {totalAmountCommand.Result}",
                    replyMarkup: Keyboards.Keyboard),

            AmountInputHandler amountInputHandler => amountInputHandler.Success
                ? new MessageView($"Amount: {user.AccountDraft.Amount}. Введите описание:")
                : new MessageView("Ошибка."),

            DescriptionInputHandler descriptionInputHandler =>
                new MessageView(
                    $"Amount: {user.AccountDraft.Amount}.\nDescription: {user.AccountDraft.Description}.\nВведите email:"),

            EmailInputHandler emailInputHandler =>
                new MessageView(
                    $"Amount: {user.AccountDraft.Amount}.\nDescription: {user.AccountDraft.Description}.\nEmail: {user.AccountDraft.Email}" +
                    $"\nКомпания {user.Company.CompanyName}. Доступные действия:", replyMarkup: Keyboards.Keyboard),

            // Свойство навигации user.Company после выполнения операции будет работать, так как найденная и присвоенная компания еще находится в контексте.
            IdInputHandler idInputHandler => idInputHandler.Success
                ? new MessageView(
                    $"Валидация ID успешно пройдена.\nКомпания {user.Company.CompanyName}. Доступные действия:",
                    replyMarkup: Keyboards.Keyboard)
                : new MessageView("Ошибка. Введите ID компании:"),

            _ => throw new ArgumentOutOfRangeException(nameof(executedHandler))
        };
    }
}