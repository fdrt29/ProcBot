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
        Commands = Utils.InstantiateAllSubclasses<Command>();

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
        User user = await Database.GetOrInsertUser(update.GetFromUniversal().Id);
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

        await _botClient.SendTextMessageAsync(update.GetMessageUniversal().Chat.Id, MessageView.GetView(handler, user));

        return Ok();
    }
}