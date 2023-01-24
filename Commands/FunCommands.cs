using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace YouTubeTestBot.Commands
{
    public class FunCommands : BaseCommandModule
    {
        [Command("message")]
        public async Task TestCommand(CommandContext ctx) 
        {
            await ctx.Channel.SendMessageAsync("Hello");
        }

        [Command("embedmessage")]
        public async Task SendEmbedMessage(CommandContext ctx) 
        {
            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "This is a title",
                Description = "This is a description",
                Color = DiscordColor.Aquamarine,
            };

            await ctx.Channel.SendMessageAsync(embed: embedMessage);
        }
    }
}
