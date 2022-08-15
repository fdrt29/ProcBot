using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Examples.WebHook;
using Telegram.Bot.Examples.WebHook.Models;
using Telegram.Bot.Examples.WebHook.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Данные хранятся в User Secrets - менеджере секретов VS
BotConfiguration? botConfig = builder.Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();

// There are several strategies for completing asynchronous tasks during startup.
// Some of them could be found in this article https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-1/
// We are going to use IHostedService to add and later remove Webhook
builder.Services.AddHostedService<ConfigureWebhook>();
builder.Services.AddDbContext<BotContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BotContext")));

// Register named HttpClient to get benefits of IHttpClientFactory
// and consume it with ITelegramBotClient typed client.
// More read:
//  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
//  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
builder.Services.AddHttpClient("tgwebhook")
    .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(botConfig.BotToken, httpClient));

// The Telegram.Bot library heavily depends on Newtonsoft.Json library to deserialize
// incoming webhook updates and send serialized responses back.
// Read more about adding Newtonsoft.Json to ASP.NET Core pipeline:
//   https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-6.0#add-newtonsoftjson-based-json-format-support
builder.Services.AddControllers().AddNewtonsoftJson();

WebApplication app = builder.Build();

app.UseRouting();
app.UseCors();

app.UseEndpoints(endpoints =>
{
    // Configure custom endpoint per Telegram API recommendations:
    // https://core.telegram.org/bots/api#setwebhook
    // If you'd like to make sure that the Webhook request comes from Telegram, we recommend
    // using a secret path in the URL, e.g. https://www.example.com/<token>.
    // Since nobody else knows your bot's token, you can be pretty sure it's us.
    string token = botConfig.BotToken;
    endpoints.MapControllerRoute("tgwebhook",
        $"bot/{token}",
        new {controller = "Webhook", action = "Post"});
    endpoints.MapControllers();
});

using (IServiceScope scope = app.Services.CreateScope())
{
    using (BotContext? context = scope.ServiceProvider.GetService<BotContext>())
    {
        //context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}

app.Run();