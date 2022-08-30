using Telegram.Bot.Examples.WebHook.Views;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Examples.WebHook;

public static class Utils
{
    public static Message GetMessageUniversal(this Update update)
    {
        return update.Type switch
        {
            UpdateType.EditedMessage => update.EditedMessage,
            UpdateType.Message => update.Message,
            UpdateType.CallbackQuery => update.CallbackQuery!.Message,
            _ => null
        } ?? throw new NotImplementedException();
    }

    public static string GetDataUniversal(this Update update)
    {
        return update.Type switch
        {
            UpdateType.EditedMessage => update.EditedMessage!.Text,
            UpdateType.Message => update.Message!.Text,
            UpdateType.CallbackQuery => update.CallbackQuery!.Data,
            _ => null
        } ?? throw new NotImplementedException();
    }

    public static User GetFromUniversal(this Update update)
    {
        return update.Type switch
        {
            UpdateType.EditedMessage => update.EditedMessage!.From,
            UpdateType.Message => update.Message!.From,
            UpdateType.CallbackQuery => update.CallbackQuery!.From,
            _ => null
        } ?? throw new NotImplementedException();
    }

    public static List<T> InstantiateAllSubclasses<T>()
    {
        IEnumerable<Type> subclasses =
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            where type.IsSubclassOf(typeof(T)) && type.IsGenericType == false && type.IsClass
            select type;

        List<T> res = new();
        foreach (Type? type in subclasses)
        {
            object instance = Activator.CreateInstance(type) ?? throw new InvalidOperationException();
            res.Add((T) instance);
        }

        return res;
    }

    public static async Task<Message> SendTextMessageAsync(this ITelegramBotClient botClient, ChatId chatId,
        MessageView messageView)
    {
        // ыыыыыыыыыыы, благо автогенерация
        (string? text, ParseMode? parseMode, IEnumerable<MessageEntity>? entities, bool? disableWebPagePreview,
            bool? disableNotification, bool? protectContent, int? replyToMessageId, bool? allowSendingWithoutReply,
            IReplyMarkup? replyMarkup) = messageView;

        return await botClient.SendTextMessageAsync(chatId, text, parseMode, entities,
            disableWebPagePreview, disableNotification, protectContent, replyToMessageId, allowSendingWithoutReply,
            replyMarkup);
    }
}