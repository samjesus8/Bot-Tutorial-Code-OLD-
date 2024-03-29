﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeTestBot.Commands.Prefix;
using YouTubeTestBot.Commands.Slash_Commands;
using YouTubeTestBot.Config;
using YouTubeTestBot.Engine;
using YouTubeTestBot.Engine.ImageHandler;
using YouTubeTestBot.Engine.LevelSystem;

namespace YouTubeTestBot
{
    public sealed class Program
    {
        //Main Discord Properties
        public static DiscordClient Client { get; set; }
        private static CommandsNextExtension Commands { get; set; }

        //Miscaleneous Properties
        private static int ImageIDCounter = 0;
        public static GoogleImageHandler imageHandler;
        private static Dictionary<string, ulong> voiceChannelIDs = new Dictionary<string, ulong>();

        static async Task Main(string[] args)
        {
            //Instantiating the class with the Instance property
            imageHandler = GoogleImageHandler.Instance;

            //Reading the Token & Prefix
            var configJson = new ConfigJSONReader();
            await configJson.ReadJSON();

            //Making a Bot Configuration with our token & additional settings
            var config = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJson.discordToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

            //Initializing the client with this config
            Client = new DiscordClient(config);

            //Setting our default timeout for Interactivity based commands
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            //EVENT HANDLERS
            Client.Ready += OnClientReady;
            Client.ComponentInteractionCreated += InteractionEventHandler;
            Client.MessageCreated += MessageSendHandler;
            Client.ModalSubmitted += ModalEventHandler;
            Client.VoiceStateUpdated += VoiceChannelHandler;
            Client.GuildMemberAdded += UserJoinHandler;

            //Setting up our Commands Configuration with our Prefix
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJson.discordPrefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
            };

            //Enabling the use of commands with our config & also enabling use of Slash Commands
            Commands = Client.UseCommandsNext(commandsConfig);
            var slashCommandsConfig = Client.UseSlashCommands();

            //Prefix Based Commands
            Commands.RegisterCommands<BasicCommands>();
            Commands.RegisterCommands<GameCommands>();
            Commands.RegisterCommands<UserRequestedCommands>();
            Commands.RegisterCommands<DiscordComponentCommands>();
            Commands.RegisterCommands<MusicCommands>();

            //Slash Commands
            slashCommandsConfig.RegisterCommands<FunSL>();
            slashCommandsConfig.RegisterCommands<ModerationSL>();

            //ERROR EVENT HANDLERS
            Commands.CommandErrored += OnCommandError;

            //Lavalink Configuration
            var endpoint = new ConnectionEndpoint
            {
                Hostname = "oce-lavalink.lexnet.cc",
                Port = 443,
                Secured = true
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "lexn3tl@val!nk",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };

            var lavalink = Client.UseLavalink();

            //Connect to the Client and get the Bot online
            await Client.ConnectAsync();
            await lavalink.ConnectAsync(lavalinkConfig);
            await Task.Delay(-1);
        }

        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        private static async Task UserJoinHandler(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            var defaultChannel = e.Guild.GetDefaultChannel();

            var welcomeEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Gold,
                Title = $"Welcome {e.Member.Username} to the server",
                Description = "Hope you enjoy your stay, please read the rules"
            };

            await defaultChannel.SendMessageAsync(embed: welcomeEmbed);
        }

        private static async Task VoiceChannelHandler(DiscordClient sender, VoiceStateUpdateEventArgs e)
        {
            var channel = e.Channel;
            var mainUserName = e.User.Username;
            if (channel != null && channel.Name == "Create" && e.Before == null) //Joining a VC
            {
                Console.WriteLine($"Joined VC {channel.Name}");

                //Creating the VC
                var userVC = await e.Guild.CreateVoiceChannelAsync($"{e.User.Username}'s Channel", e.Channel.Parent);
                voiceChannelIDs.Add(e.User.Username, userVC.Id);

                var member = await e.Guild.GetMemberAsync(e.User.Id);
                await member.ModifyAsync(x => x.VoiceChannel = userVC);
            }
            if (e.User.Username == mainUserName && channel == null && e.Before != null && e.Before.Channel != null && e.Before.Channel.Name == $"{e.User.Username}'s Channel") //Leaving the VC
            {
                Console.WriteLine($"Left the VC {e.Before.Channel.Name}");
                var channelID = voiceChannelIDs.TryGetValue(e.User.Username, out ulong channelToDelete);
                var leftChannel = e.Guild.GetChannel(channelToDelete);
                await leftChannel.DeleteAsync();

                voiceChannelIDs.Remove(e.User.Username);
            }
        }

        private static async Task ModalEventHandler(DiscordClient sender, ModalSubmitEventArgs e)
        {
            if (e.Interaction.Type == InteractionType.ModalSubmit && e.Interaction.Data.CustomId == "modal")
            {
                var values = e.Values;
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{e.Interaction.User.Username} submitted a modal with the input {values.Values.First()}"));
            }
        }

        private static async Task MessageSendHandler(DiscordClient sender, MessageCreateEventArgs e)
        {
            //Swear Filter
            var swearFilter = new SwearFilter();
            foreach (var word in swearFilter.filter)
            {
                if (e.Message.Content.Contains(word))
                {
                    await e.Channel.SendMessageAsync("That message had a swear word in it, you have been warned");
                }
            }

            //Image Counter Reset
            if (e.Message.Content == "!image")
            {
                ImageIDCounter = 0; //Reset the counter when someone uses this command
            }

            //Profile System
            var levelEngine = new LevelEngine();
            var addedXP = levelEngine.AddXP(e.Author.Username, e.Guild.Id);
            if (levelEngine.levelledUp == true)
            {
                var levelledUpEmbed = new DiscordEmbedBuilder()
                {
                    Title = e.Author.Username + " has levelled up!!!!",
                    Description = "Level: " + levelEngine.GetUser(e.Author.Username, e.Guild.Id).Level.ToString(),
                    Color = DiscordColor.Chartreuse
                };

                await e.Channel.SendMessageAsync(e.Author.Mention, embed: levelledUpEmbed);
            }
        }

        private static async Task InteractionEventHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {
            //Drop-Down Events
            if (e.Id == "dropDownList" && e.Interaction.Data.ComponentType == ComponentType.StringSelect)
            {
                var options = e.Values;
                foreach (var option in options)
                {
                    switch (option)
                    {
                        case "option1":
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"{e.User.Username} has selected Option 1"));
                            break;

                        case "option2":
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"{e.User.Username} has selected Option 2"));
                            break;

                        case "option3":
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"{e.User.Username} has selected Option 3"));
                            break;
                    }
                }
            }
            else if (e.Id == "channelDropDownList")
            {
                var options = e.Values;
                foreach (var channel in options)
                {
                    var selectedChannel = await Client.GetChannelAsync(ulong.Parse(channel));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{e.User.Username} selected the channel with name {selectedChannel.Name}"));
                }
            }

            else if (e.Id == "mentionDropDownList")
            {
                var options = e.Values;
                foreach (var user in options)
                {
                    var selectedUser = await Client.GetUserAsync(ulong.Parse(user));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"{selectedUser.Mention} was mentionned"));
                }
            }

            //Button Events
            if (e.Interaction.Data.CustomId == "1")
            {
                var options = e.Values;
                foreach (var option in options)
                {
                    switch (option)
                    {
                        case "option1":
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("You selected Option1"));
                            break;

                        case "option2":
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("You selected Option2"));
                            break;

                        case "option3":
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("You selected Option3"));
                            break;

                        default:
                            Console.WriteLine("Error in Interaction Event Handler");
                            break;
                    }
                }
            }

            //Buttons
            switch (e.Interaction.Data.CustomId)
            {
                case "1":
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("You pressed the 1st Button"));
                    break;

                case "2":
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("You pressed the 2nd Button"));
                    break;

                case "funButton":
                    string funCommandsList = "!message -> Send a message \n" +
                         "!embedmessage1 -> Sends an embed message \n" +
                         "!poll -> Starts a poll";

                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent(funCommandsList));

                    break;

                case "gameButton":
                    string gamesList = "!cardgame -> Play a simple card game. Whoever draws the highest wins the game";

                    var gamesCommandList = new DiscordEmbedBuilder()
                    {
                        Title = gamesList,
                    };

                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(gamesCommandList));

                    break;

                case "previousButton":
                    ImageIDCounter--; //Decrement the ID by 1 to get the ID for the previous image
                    string imageURL = Program.imageHandler.GetImageAtId(ImageIDCounter); //Get the image from the Dictionary

                    //Initialise the Buttons again

                    var previousEmoji = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":track_previous:"));
                    var previousButton = new DiscordButtonComponent(ButtonStyle.Primary, "previousButton", "Previous", false, previousEmoji);

                    var nextEmoji = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":track_next:"));
                    var nextButton = new DiscordButtonComponent(ButtonStyle.Primary, "nextButton", "Next", false, nextEmoji);

                    //Send the new image as a response to the button press, replacing the previous image

                    var imageMessage = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.Azure)
                        .WithTitle("Results")
                        .WithImageUrl(imageURL)
                        .WithFooter("Page " + ImageIDCounter)
                        )
                        .AddComponents(previousButton, nextButton);
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(imageMessage.Embed).AddComponents(imageMessage.Components));

                    break;

                case "nextButton":
                    ImageIDCounter++; //Same idea but this time you increment the counter by 1 to get the next image
                    string imageURL1 = Program.imageHandler.GetImageAtId(ImageIDCounter);

                    var previousEmoji1 = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":track_previous:"));
                    var previousButton1 = new DiscordButtonComponent(ButtonStyle.Primary, "previousButton", "Previous", false, previousEmoji1);

                    var nextEmoji1 = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":track_next:"));
                    var nextButton1 = new DiscordButtonComponent(ButtonStyle.Primary, "nextButton", "Next", false, nextEmoji1);

                    var imageMessage1 = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.Azure)
                        .WithTitle("Results")
                        .WithImageUrl(imageURL1)
                        .WithFooter("Page " + ImageIDCounter)
                        )
                        .AddComponents(previousButton1, nextButton1);
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(imageMessage1.Embed).AddComponents(imageMessage1.Components));

                    break;

                default:
                    Console.WriteLine("No Buttons were found with this ID");
                    break;
            }
        }

        private static async Task OnCommandError(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            //Casting my ErrorEventArgs as a ChecksFailedException
            if (e.Exception is ChecksFailedException castedException)
            {
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
