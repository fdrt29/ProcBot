using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Examples.WebHook.Models;
using Telegram.Bot.Examples.WebHook.Models.Abstract;
using Telegram.Bot.Examples.WebHook.Models.Commands.Abstract;
using Telegram.Bot.Examples.WebHook.Models.UserInputHandlers.Abstract;
using Telegram.Bot.Examples.WebHook.Views;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = Telegram.Bot.Examples.WebHook.Models.User;

namespace Telegram.Bot.Examples.WebHook.Controllers;

public class WebhookController : Controller
{
    private static ITelegramBotClient _botClient;

    private static readonly List<Command>
        Commands = Utils.InstantiateAllSubclasses<Command>(); // TODO mb find better place to store commands

    private static readonly List<InputHandler> InputHandlers = Utils.InstantiateAllSubclasses<InputHandler>();
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(ITelegramBotClient botClient, ILogger<WebhookController> logger, BotContext database)
    {
        _botClient = botClient;
        _logger = logger;
        Database = database;
    }

    private BotContext Database { get; }

    protected override void Dispose(bool disposing)
    {
        Database.Dispose();
        base.Dispose(disposing);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        IActionResult res = Ok();
        try
        {
            res = await Do(update);
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            Console.WriteLine(e);
        }

        return res;
    }


    public async Task<IActionResult> Do(Update update)
    {
        // TODO потенциально затычка асинхронности
        // TODO mb создавать по новому объекту комманды для запроса и ставить их на очередь пула потоков

        User user = await Database.InsertUserIfNotExist(update.GetFromUniversal().Id);
        IHandler? handler = null;
        // Запуск цепочки ответственности
        if (update.GetDataUniversal().StartsWith("/"))
            foreach (Command command in Commands)
            {
                if (!command.IsMatch(update, user)) continue;

                await command.Execute(update, Database, user);
                handler = command;
                break;
            }
        else
            foreach (InputHandler inputHandler in InputHandlers)
            {
                if (!inputHandler.IsMatch(update, user)) continue;

                await inputHandler.Execute(update, Database, user);
                handler = inputHandler;
                break;
            }

        if (handler == null) return Ok();
        // ыыыыыыыыыыы, благо автогенерация
        (string? text, ParseMode? parseMode, IEnumerable<MessageEntity>? entities, bool? disableWebPagePreview,
            bool? disableNotification, bool? protectContent, int? replyToMessageId, bool? allowSendingWithoutReply,
            IReplyMarkup? replyMarkup) = Helpers.GetView(handler, user);

        await _botClient.SendTextMessageAsync(update.GetMessageUniversal().Chat.Id, text, parseMode, entities,
            disableWebPagePreview, disableNotification, protectContent, replyToMessageId, allowSendingWithoutReply,
            replyMarkup);
        
        //return View(); // TODO можно посмотреть получится ли настроить поведение WebhookController так, чтобы возвращаемым View была строка сообщения
        return Ok();
    }
}