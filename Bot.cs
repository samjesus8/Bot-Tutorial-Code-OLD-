using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using YouTubeTestBot.Commands;
using YouTubeTestBot.Config;
using YouTubeTestBot.Slash_Commands;

namespace YouTubeTestBot
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync() 
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            var configJson = JsonConvert.DeserializeObject<ConfigJSON>(json);

            var config = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

            Client = new DiscordClient(config);
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            Client.Ready += OnClientReady;
            Client.ComponentInteractionCreated += ButtonPressResponse;

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
            };

            Commands = Client.UseCommandsNext(commandsConfig);
            var slashCommandsConfig = Client.UseSlashCommands();

            //Prefix Based Commands
            Commands.RegisterCommands<FunCommands>();
            Commands.RegisterCommands<GameCommands>();

            //Slash Commands
            slashCommandsConfig.RegisterCommands<FunSL>(1015010557591572560);
            slashCommandsConfig.RegisterCommands<ModerationSL>(1015010557591572560);

            Commands.CommandErrored += OnCommandError;

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private async Task ButtonPressResponse(DiscordClient sender, DSharpPlus.EventArgs.ComponentInteractionCreateEventArgs e)
        {
            if (e.Interaction.Data.CustomId == "1") 
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You pressed the 1st Button"));
            }
            else if (e.Interaction.Data.CustomId == "2") 
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("You pressed the 2nd Button"));
            }

            else if (e.Interaction.Data.CustomId == "funButton") 
            {
                string funCommandsList = "!message -> Send a message \n" +
                                         "!embedmessage1 -> Sends an embed message \n" +
                                         "!poll -> Starts a poll";

                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent(funCommandsList));
            }
            else if (e.Interaction.Data.CustomId == "gameButton") 
            {
                string gamesList = "!cardgame -> Play a simple card game. Whoever draws the highest wins the game";

                var gamesCommandList = new DiscordInteractionResponseBuilder()
                {
                    Title = "Game Command List",
                    Content = gamesList,
                };

                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, gamesCommandList);
            }
        }

        private Task OnClientReady(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        private async Task OnCommandError(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            if (e.Exception is ChecksFailedException) 
            {
                var castedException = (ChecksFailedException)e.Exception; //Casting my ErrorEventArgs as a ChecksFailedException
                string cooldownTimer = string.Empty;

                foreach (var check in castedException.FailedChecks)
                {
                    var cooldown = (CooldownAttribute)check; //The cooldown that has triggered this method
                    TimeSpan timeLeft = cooldown.GetRemainingCooldown(e.Context); //Getting the remaining time on this cooldown
                    cooldownTimer = timeLeft.ToString(@"hh\:mm\:ss");
                }

                var cooldownMessage = new DiscordEmbedBuilder()
                {
                    Title = "Wait for the Cooldown to End",
                    Description = "Remaining Time: " + cooldownTimer,
                    Color = DiscordColor.Red
                };

                await e.Context.Channel.SendMessageAsync(embed: cooldownMessage);
            }
        }
    }
}
