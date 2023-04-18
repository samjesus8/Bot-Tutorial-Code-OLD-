using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System;
using System.Threading.Tasks;
using System.Timers;
using YouTubeTestBot.Commands;
using YouTubeTestBot.Config;
using YouTubeTestBot.Engine.ImageHandler;
using YouTubeTestBot.Engine.LevelSystem;
using YouTubeTestBot.Engine.YouTube;
using YouTubeTestBot.Slash_Commands;

namespace YouTubeTestBot
{
    public sealed class Program
    {
        //Main Discord Properties
        private static DiscordClient Client { get; set; }
        private static InteractivityExtension Interactivity { get; set; }
        private static CommandsNextExtension Commands { get; set; }

        //YouTube Properties
        private static YouTubeVideo _video = new YouTubeVideo();
        private static YouTubeVideo temp = new YouTubeVideo();
        private static YouTubeEngine _YouTubeEngine = new YouTubeEngine();

        //Miscaleneous Properties
        private static int ImageIDCounter = 0;
        public static GoogleImageHandler imageHandler;

        static async Task Main(string[] args)
        {
            imageHandler = GoogleImageHandler.Instance;

            var configJson = new ConfigJSONReader();
            await configJson.ReadJSON();

            var config = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJson.token,
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
            Client.MessageCreated += MessageSendHandler;

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJson.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
            };

            Commands = Client.UseCommandsNext(commandsConfig);
            var slashCommandsConfig = Client.UseSlashCommands();

            //Prefix Based Commands
            Commands.RegisterCommands<BasicCommands>();
            Commands.RegisterCommands<GameCommands>();
            Commands.RegisterCommands<UserRequestedCommands>();

            //Slash Commands
            slashCommandsConfig.RegisterCommands<FunSL>();
            slashCommandsConfig.RegisterCommands<ModerationSL>();

            Commands.CommandErrored += OnCommandError;

            await Client.ConnectAsync();

            ulong channelIdToNotify = 1088762895573209178; // your Discord channel ID
            await StartVideoUploadNotifier(_YouTubeEngine.channelId, _YouTubeEngine.apiKey, Client, channelIdToNotify);
            await Task.Delay(-1);
        }

        private static async Task MessageSendHandler(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Message.Content == "!image")
            {
                ImageIDCounter = 0; //Reset the counter when someone uses this command
            }

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

        private static async Task ButtonPressResponse(DiscordClient sender, ComponentInteractionCreateEventArgs e)
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
            else if (e.Interaction.Data.CustomId == "previousButton")
            {
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
            }
            else if (e.Interaction.Data.CustomId == "nextButton")
            {
                ImageIDCounter++; //Same idea but this time you increment the counter by 1 to get the next image
                string imageURL = Program.imageHandler.GetImageAtId(ImageIDCounter);

                var previousEmoji = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":track_previous:"));
                var previousButton = new DiscordButtonComponent(ButtonStyle.Primary, "previousButton", "Previous", false, previousEmoji);

                var nextEmoji = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":track_next:"));
                var nextButton = new DiscordButtonComponent(ButtonStyle.Primary, "nextButton", "Next", false, nextEmoji);

                var imageMessage = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Azure)
                    .WithTitle("Results")
                    .WithImageUrl(imageURL)
                    .WithFooter("Page " + ImageIDCounter)
                    )
                    .AddComponents(previousButton, nextButton);
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(imageMessage.Embed).AddComponents(imageMessage.Components));
            }
        }

        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        private static async Task OnCommandError(CommandsNextExtension sender, CommandErrorEventArgs e)
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

        private static async Task StartVideoUploadNotifier(string channelId, string apiKey, DiscordClient client, ulong channelIdToNotify)
        {
            var timer = new Timer(120000); //Timer set for 2 min
            timer.Elapsed += async (sender, e) => {
                _video = _YouTubeEngine.GetLatestVideo(channelId, apiKey); //Get latest video using API
                DateTime lastCheckedAt = DateTime.Now;

                if (_video != null)
                {
                    if (temp.videoTitle == _video.videoTitle) //This ensures that only the newest videos get sent through
                    {
                        Console.WriteLine("Same name");
                    }
                    else if (_video.PublishedAt < lastCheckedAt) //If the new video is actually new
                    {
                        var message = $"NEW VIDEO | **{_video.videoTitle}** \n" +
                                      $"Published at: {_video.PublishedAt} \n" +
                                      "URL: " + _video.videoUrl;

                        await client.GetChannelAsync(channelIdToNotify).Result.SendMessageAsync(message);
                        temp = _video;
                    }
                    else
                    {
                        Console.WriteLine("[" +lastCheckedAt.ToString()+ "]" + "YouTube API: No new videos were found");
                    }
                }
            };
            timer.Start();
        }
    }
}
