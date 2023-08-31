using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using YouTubeTestBot.Engine.LevelSystem;

namespace YouTubeTestBot.Commands.Prefix
{
    public class BasicCommands : BaseCommandModule
    {
        [Command("message")]
        [Cooldown(5, 10, CooldownBucketType.User)]
        public async Task TestCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Test Message");
        }

        [Command("embedmessage1")]
        public async Task SendEmbedMessage2(CommandContext ctx) //Example 1
        {
            var embedMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Azure)
                    .WithTitle("This is a title")
                    .WithDescription("This is a description"));

            await ctx.Channel.SendMessageAsync(embedMessage);
        }

        [Command("embedmessage2")]
        public async Task SendEmbedMessage1(CommandContext ctx) //Example 2
        {
            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "This is a title",
                Description = "This is a description",
                Color = DiscordColor.Azure,
            };

            await ctx.Channel.SendMessageAsync(embed: embedMessage);
        }

        [Command("help")]
        public async Task HelpCommand(CommandContext ctx) 
        {
            var funButton = new DiscordButtonComponent(ButtonStyle.Success, "funButton", "Fun");
            var gameButton = new DiscordButtonComponent(ButtonStyle.Success, "gameButton", "Games");

            var helpMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Azure)
                    .WithTitle("Help Menu")
                    .WithDescription("Please pick a button for more information on the commands"))
                .AddComponents(funButton, gameButton);

            await ctx.Channel.SendMessageAsync(helpMessage);
        }

        [Command("profile")]
        public async Task ProfileCommand(CommandContext ctx)
        {
            string username = ctx.User.Username;
            ulong guildID = ctx.Guild.Id;

            var userDetails = new DUser()
            {
                UserName = ctx.User.Username,
                guildID = ctx.Guild.Id,
                avatarURL = ctx.User.AvatarUrl,
                Level = 1,
                XP = 0
            };

            var levelEngine = new LevelEngine();
            var doesExist = levelEngine.CheckUserExists(username, guildID);

            if (doesExist == false)
            {
                var isStored = levelEngine.StoreUserDetails(userDetails);
                if (isStored == true)
                {
                    var successMessage = new DiscordEmbedBuilder()
                    {
                        Title = "Succesfully created profile",
                        Description = "Please execute !profile again to view your profile",
                        Color = DiscordColor.Green
                    };

                    await ctx.Channel.SendMessageAsync(embed: successMessage);

                    var pulledUser = levelEngine.GetUser(username, guildID);

                    var profile = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithColor(DiscordColor.Aquamarine)
                            .WithTitle(pulledUser.UserName + "'s Profile")
                            .WithThumbnail(pulledUser.avatarURL)
                            .AddField("Level", pulledUser.Level.ToString(), true)
                            .AddField("XP", pulledUser.XP.ToString(), true)
                        );

                    await ctx.Channel.SendMessageAsync(profile);
                }
                else
                {
                    var failedMessage = new DiscordEmbedBuilder()
                    {
                        Title = "Something went wrong when creating your profile",
                        Color = DiscordColor.Red
                    };

                    await ctx.Channel.SendMessageAsync(embed: failedMessage);
                }
            }
            else
            {
                var pulledUser = levelEngine.GetUser(username, guildID);

                var profile = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.Aquamarine)
                        .WithTitle(pulledUser.UserName + "'s Profile")
                        .WithThumbnail(pulledUser.avatarURL)
                        .AddField("Level", pulledUser.Level.ToString(), true)
                        .AddField("XP", pulledUser.XP.ToString(), true)
                    );

                await ctx.Channel.SendMessageAsync(profile);
            }
        }
    }
}
