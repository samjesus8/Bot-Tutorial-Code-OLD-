using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace YouTubeTestBot.Commands
{
    public class BasicCommands : BaseCommandModule
    {
        [Command("message")]
        [Cooldown(5, 10, CooldownBucketType.User)]
        public async Task TestCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("@𝕤𝕒𝕞.𝕛𝕖𝕤𝕦𝕤𝟠#6825");
        }

        [Command("embedmessage1")]
        public async Task SendEmbedMessage2(CommandContext ctx) //Example 1
        {
            var fieldEmbed = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .AddField("Field1", "This is a field", true)
                .AddField("Field2", "This is a field", true)
                .AddField("Field3", "This is a field", true)
                .AddField("Field4", "This is a field", true)
                .WithColor(DiscordColor.Azure)
                );

            var embedMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                .WithColor(DiscordColor.Azure)
                .WithTitle("This is a title")
                .WithDescription("This is a description")
                );

            await ctx.Channel.SendMessageAsync(fieldEmbed);
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

        [Command("restrictions")]

        //For any Restrictions here, make sure to register an event in your CommandErrored Method to handle this

        //Restricting a Command by Roles
        [RequireRoles(RoleCheckMode.MatchNames, "Enter Your Roles Here")]

        //Restricting a Command to specific permissions
        [RequireBotPermissions(DSharpPlus.Permissions.Administrator, true)]

        //Restricting a Command to Owner
        [RequireOwner]
        public async Task CommandRestrictionExamples(CommandContext ctx) 
        {
            //Restricting a Command by Server
            if (ctx.Guild.Id == 1015010557591572560) 
            {
                //Your Server ID can be found by right-clicking on your Server Icon and clicking "Copy ID"
            }

            //Restricting a Command by Channel
            if (ctx.Channel.Id == 1017524740610592808) 
            {
                //Right click on the channel you want this command ONLY to execute and click "Copy ID"
            }

            //Restricting a Command to NSFW Channels
            if (ctx.Channel.IsNSFW == true) 
            {
                //You are checking if this is being executed in an NSFW channel, so your ELSE block must send an error message elsewhere
            }

            //Restricting a Command to a specific user
            if (ctx.User.Id == 01234 || ctx.User.Username == "Enter Your Username Here") 
            {
                //You can check for the user by their ID or their Username
            }
        }

        [Command("button")]
        public async Task ButtonExample(CommandContext ctx) 
        {
            DiscordButtonComponent button1 = new DiscordButtonComponent(ButtonStyle.Primary, "1", "Button 1");
            DiscordButtonComponent button2 = new DiscordButtonComponent(ButtonStyle.Primary, "2", "Button 2");

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                .WithColor(DiscordColor.Azure)
                .WithTitle("This is a message with buttons")
                .WithDescription("Please select a button")
                )
                .AddComponents(button1)
                .AddComponents(button2);

            await ctx.Channel.SendMessageAsync(message);
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
                .WithDescription("Please pick a button for more information on the commands")
                )
                .AddComponents(funButton, gameButton);

            await ctx.Channel.SendMessageAsync(helpMessage);
        }
    }
}
