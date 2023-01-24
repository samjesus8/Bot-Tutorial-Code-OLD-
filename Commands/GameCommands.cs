using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using YouTubeTestBot.External_Classes;

namespace YouTubeTestBot.Commands
{
    public class GameCommands : BaseCommandModule
    {
        [Command("cardgame")]
        public async Task SimpleCardGame(CommandContext ctx) 
        {
            var UserCard = new CardBuilder();

            var userCardMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                .WithColor(DiscordColor.Azure)
                .WithTitle("Your Card")
                .WithDescription("You drew a: " + UserCard.SelectedCard)
                );

            await ctx.Channel.SendMessageAsync(userCardMessage);

            var BotCard = new CardBuilder();

            var botCardMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                
                .WithColor(DiscordColor.Azure)
                .WithTitle("Bot Card")
                .WithDescription("The Bot drew a: " + BotCard.SelectedCard)
                );

            await ctx.Channel.SendMessageAsync(botCardMessage);

            if (UserCard.SelectedNumber > BotCard.SelectedNumber) 
            {
                //The user wins
                var winningMessage = new DiscordEmbedBuilder()
                {
                    Title = "**You Win the game!!**",
                    Color = DiscordColor.Green
                };

                await ctx.Channel.SendMessageAsync(embed: winningMessage);
                return;
            }
            else
            {
                //The bot wins
                var losingMessage = new DiscordEmbedBuilder()
                {
                    Title = "**You Lost the game**",
                    Color = DiscordColor.Red
                };

                await ctx.Channel.SendMessageAsync(embed: losingMessage);
                return;
            }
        }
    }
}
