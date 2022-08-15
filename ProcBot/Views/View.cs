using Telegram.Bot.Examples.WebHook.Models.Abstract;
using Telegram.Bot.Examples.WebHook.Models.Commands;
using Telegram.Bot.Examples.WebHook.Models.Commands.CreateAccount;
using Telegram.Bot.Examples.WebHook.Models.UserInputHandlers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = Telegram.Bot.Examples.WebHook.Models.User;

namespace Telegram.Bot.Examples.WebHook.Views;

public struct View
{
    public View(string text, ParseMode? parseMode = default, IEnumerable<MessageEntity>? entities = default,
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

    public string Text;
    public ParseMode? ParseMode;
    public IEnumerable<MessageEntity>? Entities;
    public bool? DisableWebPagePreview;
    public bool? DisableNotification;
    public bool? ProtectContent;
    public int? ReplyToMessageId;
    public bool? AllowSendingWithoutReply;
    public IReplyMarkup? ReplyMarkup;
}

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

public static class Helpers
{
    public static View GetView(IHandler executedHandler, User user)
    {
        return executedHandler switch
        {
            StartCommand startCommand =>
                new View("Hallo I'm ASP.NET Core Bot", ParseMode.Markdown, replyMarkup: Keyboards.StartKeyboard),


            CreateAccountCommand createAccountCommand =>
                new View("Создание счета. Введите сумму:"),


            LastOperationsCommand lastOperationsCommand =>
                new View("Последние 10 операций:\n" + lastOperationsCommand.Result + "\nВыберите действие:",
                    replyMarkup: Keyboards.Keyboard),
            // TODO возвращать массив пар строк и клавиатур? Чтобы можно было несколько сообщений в ответ на одну команду отправлять
            // await botClient.SendTextMessageAsync(chat, "Последние 10 операций:\n" + message);
            // await botClient.SendTextMessageAsync(chat, "Выберите действие:",
            // replyMarkup: Keyboards.Keyboard);


            LoginCommand loginCommand =>
                new View("Введите ID компании:"),


            TotalAmountCommand totalAmountCommand =>
                new View($"Общая сумма операций за день: {totalAmountCommand.Result}", replyMarkup: Keyboards.Keyboard),
            // TODO выяснить не будет ли проблемы с асинхронностью? Сейчас по одному экземпляру команды на тип, не может ли быть конкуренция за поле .Result в них?


            AmountInputHandler amountInputHandler => amountInputHandler.Success
                ? new View($"Amount: {user.AccountDraft.Amount}. Введите описание:")
                : new View("Ошибка."),
            // //TODO edit text of previous message in order to display entered amount
            // // TODO сообщение:
            // // Amount: %эиодзи%
            // // Desctiption: %эиодзи%
            // // Email: %эиодзи%
            // // Разные эмодзи для: еще невведеных данных, отправлденных невалидных данных, принятых
            // // await botClient.EditMessageTextAsync(message.Chat.Id, message.MessageId, $"Amount: {amount}");
            // await botClient.SendTextMessageAsync(message.Chat.Id, $"Amount: {amount}. Введите описание:");
            //
            //             await botClient.SendTextMessageAsync(message.Chat, "Ошибка.");


            DescriptionInputHandler descriptionInputHandler =>
                new View(
                    $"Amount: {user.AccountDraft.Amount}.\nDescription: {user.AccountDraft.Description}.\nВведите email:"),


            EmailInputHandler emailInputHandler =>
                new View(
                    $"Amount: {user.AccountDraft.Amount}.\nDescription: {user.AccountDraft.Description}.\nEmail: {user.AccountDraft.Email}" +
                    $"\nКомпания {user.Company.CompanyName}. Доступные действия:", replyMarkup: Keyboards.Keyboard),
            // await botClient.SendTextMessageAsync(message.Chat.Id,
            // $"Amount: {user.AccountDraft.Amount}.\nDescription: {user.AccountDraft.Description}.\nEmail: {email}");
            // // TODO вызвать просто команду с таким текстом?
            // await botClient.SendTextMessageAsync(message.Chat, $"Компания {company?.CompanyName}. Доступные действия:",
            // replyMarkup: Keyboards.Keyboard);

            // Свойство связи user.Company после выполнения операции будет работать, так как найденная и присвоенная компания еще находится в контексте.
            IdInputHandler idInputHandler => idInputHandler.Success
                ? new View($"Валидация ID успешно пройдена.\nКомпания {user.Company.CompanyName}. Доступные действия:",
                    replyMarkup: Keyboards.Keyboard)
                : new View("Ошибка. Введите ID компании:"),
            //await botClient.SendTextMessageAsync(message.Chat, "Валидация ID успешно пройдена.");
            // await botClient.SendTextMessageAsync(message.Chat,
            // $"Компания {company?.CompanyName}. Доступные действия:", replyMarkup: Keyboards.Keyboard);


            _ => throw new ArgumentOutOfRangeException(nameof(executedHandler))
        };
    }
}