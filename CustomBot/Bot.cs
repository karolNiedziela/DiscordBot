using CustomBot.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBot
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }

        public InteractivityExtension Interactivity { get; private set; }

        public VoiceNextExtension Voice { get; private set; }

        public CommandsNextExtension Commands { get; set; }

        private IServiceProvider _serviceProvider;

        public Bot(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task RunAsync()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead(Path.GetFullPath(@"C:\Users\niedz\Desktop\C#\CustomBot\CustomBot\bin\Debug\net5.0\config.json")))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug
            };

            var endpoint = new ConnectionEndpoint
            {
                Hostname = "127.0.0.1",
                Port = 2333
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "youshallnotpass",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableDms = false,
                EnableMentionPrefix = true,
                DmHelp = false,
                Services = _serviceProvider
            };

            var lavalink = Client.UseLavalink();

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<PlaylistCommands>();
            Commands.RegisterCommands<FounderCommands>();
            Commands.RegisterCommands<JavalinkCommands>();

            Voice = Client.UseVoiceNext();

            await Client.ConnectAsync();
            await lavalink.ConnectAsync(lavalinkConfig);

            await Task.Delay(-1);
        }

        private Task OnClientReady(object sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}