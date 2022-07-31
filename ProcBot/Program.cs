using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace ProcBot;

public enum State
{
    Unidentified,
    WaitId,
    Identified,
    CreateBill,

    // Bill creating states
    WaitAmount,
    WaitDescription,
    WaitEmail
}

public enum Command
{
    Identify,
    CreateAccount,
    GetLastOperations,
    GetPaymentsAmountPerDay,
    Exit
}

// TODO либо логиниться с id и паролем как компания, либо вместо id
internal static class Program
{
    // private static readonly string ConfigPath = "config.json";

    public static readonly Configuration Configuration =
        JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(@"config.json"));

    public static readonly UserContext Database = new();

    // TODO parse token from local secure json file. Mb make secret config and open config
    private static readonly ITelegramBotClient Bot = new TelegramBotClient(Configuration.Token);

    private static void Main(string[] args)
    {
        string name = Console.Title = Bot.GetMeAsync().Result.FirstName;
        Console.WriteLine("Запущен бот " + name);

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;
        ReceiverOptions receiverOptions = new();

        Bot.StartReceiving(
            Handlers.HandleUpdateAsync,
            Handlers.HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );
        Console.ReadLine();
        cts.Cancel();
    }
}