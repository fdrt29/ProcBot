using Telegram.Bot.Types.ReplyMarkups;

namespace ProcBot
{
    public class Keyboards
    {
        public static readonly InlineKeyboardMarkup StartKeyboard = new[]
        {
            InlineKeyboardButton.WithCallbackData("Login", "/login")
        };

        public static readonly InlineKeyboardMarkup Keyboard = new[]
        {
            InlineKeyboardButton.WithCallbackData("Create bill", "/createBill"),
            InlineKeyboardButton.WithCallbackData("Last 10 operations", "/lastOperations"),
            InlineKeyboardButton.WithCallbackData("Amount of payments per day", "/amount")
        };
    }
}