using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.IO;

public class MainLoop {
    // Program entry point
    static void Main(string[] args) { 
        new MainLoop().MainAsync().GetAwaiter().GetResult();
    }

    private readonly DiscordSocketClient _client; 
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;

    private MainLoop() {
        _client = new DiscordSocketClient(new DiscordSocketConfig {
            // How much logging do you want to see?
            LogLevel = LogSeverity.Info, 
            MessageCacheSize = 50
        });

        _commands = new CommandService(new CommandServiceConfig { 
            LogLevel = LogSeverity.Info, 
            CaseSensitiveCommands = false,
        });
         
        _client.Log += Log;
        _commands.Log += Log;
         
        _services = ConfigureServices();

    } 
    private static IServiceProvider ConfigureServices() {
        var map = new ServiceCollection(); 
        return map.BuildServiceProvider();
    }
     
    private static Task Log(LogMessage message) {
        switch (message.Severity) {
            case LogSeverity.Critical:
            case LogSeverity.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogSeverity.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogSeverity.Info:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogSeverity.Verbose:
            case LogSeverity.Debug:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
        }
        Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
        Console.ResetColor();

        return Task.CompletedTask;
    }

    private async Task MainAsync() { 
        await InitCommands();
         
        await _client.LoginAsync(TokenType.Bot, File.ReadAllText("token.secret"));
        await _client.StartAsync();
         
        await Task.Delay(Timeout.Infinite);
    }

    private async Task InitCommands() {
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _client.MessageReceived += HandleCommandAsync;
    }

    private async Task HandleCommandAsync(SocketMessage arg) { 
        var msg = arg as SocketUserMessage;
        if (msg == null) return;
         
        if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;
         
        int pos = 0;
        if (msg.HasCharPrefix('!', ref pos)) {
            var context = new SocketCommandContext(_client, msg);

            var result = await _commands.ExecuteAsync(context, pos, _services);
        }
    }
}
