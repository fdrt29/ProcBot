using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.WebHook.Models.Abstract;

// Класс обработчика из цепочки ответственности. Сам определяет должен ли он обрабатывать обновление
public interface IHandler
{
    public Task Execute(Update update, BotContext db, User user);
    public bool IsMatch(Update update, User user);
}