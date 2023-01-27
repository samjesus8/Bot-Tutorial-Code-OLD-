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
            var UserCard = new CardBuilder(); //Creating an instance of a card for the user

            var userCardMessage = new DiscordMessageBuilder() //Displaying the User's card in an embed
                .AddEmbed(new DiscordEmbedBuilder()

                .WithColor(DiscordColor.Azure)
                .WithTitle("Your Card")
                .WithDescription("You drew a: " + UserCard.SelectedCard)
                );

            await ctx.Channel.SendMessageAsync(userCardMessage);

            var BotCard = new CardBuilder(); //Creating an instance of a card for the Bot

            var botCardMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                
                .WithColor(DiscordColor.Azure)
                .WithTitle("Bot Card")
                .WithDescription("The Bot drew a: " + BotCard.SelectedCard)
                );

            await ctx.Channel.SendMessageAsync(botCardMessage);

            if (UserCard.SelectedNumber > BotCard.SelectedNumber) //Comparing the two cards
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
