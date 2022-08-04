using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ProcBot
{
    //TODO mb decompose handlers to state-objects
    public class Handlers
    {
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(JsonConvert.SerializeObject(update));

            Task handler = update.Type switch
            {
                // UpdateType.Unknown => expr,
                UpdateType.Message => HandleMessageAsync(botClient, update.Message, cancellationToken),
                // UpdateType.InlineQuery => expr,
                // UpdateType.ChosenInlineResult => expr,
                UpdateType.CallbackQuery =>
                    HandleCallbackQueryAsync(botClient, update.CallbackQuery, cancellationToken),
                UpdateType.EditedMessage => HandleMessageAsync(botClient, update.EditedMessage, cancellationToken),
                // UpdateType.ChannelPost => expr,
                // UpdateType.EditedChannelPost => expr,
                // UpdateType.ShippingQuery => expr,
                // UpdateType.PreCheckoutQuery => expr,
                // UpdateType.Poll => expr,
                // UpdateType.PollAnswer => expr,
                // UpdateType.MyChatMember => expr,
                // UpdateType.ChatMember => expr,
                // UpdateType.ChatJoinRequest => expr,
                _ => HandleDefualtAsync(botClient, update, cancellationToken)
            };

            try
            {
                await handler;
            }
            catch (Exception e)
            {
                await HandleErrorAsync(botClient, e, cancellationToken);
            }
        }

        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(JsonConvert.SerializeObject(exception));
            Console.ResetColor();
            // TODO mb logger
            return Task.CompletedTask;
        }

        public static async Task HandleDefualtAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
        }

        public static async Task HandleMessageAsync(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            if (message.From is null) return;
            Models.User user = await Program.Database.Users.FindAsync(message.From.Id);
            if (message.Text?.ToLower() == "/start")
            {
                Task<Message> sendTask = botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать!",
                    cancellationToken: cancellationToken, replyMarkup: Keyboards.StartKeyboard);
                // Добавление пользователя, если его нет в бд, или сброс в начальное состояние
                if (user is null)
                {
                    user = new Models.User
                    {
                        UserId = message.From.Id,
                        CompanyId = null,
                        State = State.Unidentified
                    };
                    Program.Database.Users.Add(user);
                }
                else
                {
                    user.State = State.Unidentified;
                }

                await Program.Database.SaveChangesAsync(cancellationToken);
                await sendTask;
                return;
            }

            if (user?.State == State.WaitId)
            {
                // TODO
                if (await ValidId(message.Text) == true)
                {
                    user.State = State.Identified;
                    await Program.Database.SaveChangesAsync(cancellationToken);

                    await botClient.SendTextMessageAsync(message.Chat, "Вход выполнен. Доступные действия",
                        cancellationToken: cancellationToken, replyMarkup: Keyboards.Keyboard);
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Ошибка. Введите ID компании.",
                        cancellationToken: cancellationToken);
                }
            }
        }

        private static async ValueTask<bool> ValidId(string messageText)
        {
            int id = Int32.Parse(messageText);
            ValueTask<Models.Company> res = Program.Database.Companies.FindAsync(id);
            if (id > 0 && await res != null) return true;
            return false;
        }

        public static async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
            CancellationToken cancellationToken)
        {
            if (callbackQuery.Message == null) return; // TODO

            switch (callbackQuery.Data)
            {
                case "/login":
                    Models.User user = await Program.Database.Users.FindAsync(callbackQuery.From.Id);
                    if (user?.State != State.Unidentified) return;

                    user.State = State.WaitId;
                    await Program.Database.SaveChangesAsync(cancellationToken);
                    // TODO переместить отправку сообщений в отдельный поток?
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Введите ID компании.",
                        cancellationToken: cancellationToken /*, replyMarkup: Keyboards.Keyboard*/);
                    // TODO to waiting state
                    return;
                case "/createBill":
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Тык-тык!",
                        cancellationToken: cancellationToken, replyMarkup: Keyboards.Keyboard);
                    return;
                case "/lastOperations":
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Тык-тык!",
                        cancellationToken: cancellationToken, replyMarkup: Keyboards.Keyboard);
                    return;
                case "/amount":
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Тык-тык!",
                        cancellationToken: cancellationToken, replyMarkup: Keyboards.Keyboard);
                    return;
            }

            // await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Привет-привет!!",
            //     cancellationToken: cancellationToken, replyMarkup: Keyboard);
        }
    }
}