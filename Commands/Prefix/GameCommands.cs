using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Threading.Tasks;
using YouTubeTestBot.Engine.External_Classes;

namespace YouTubeTestBot.Commands.Prefix
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

        [Command("poll")]
        public async Task Poll(CommandContext ctx, string option1, string option2, string option3, string option4, [RemainingText] string pollTitle)
        {
            var interactivity = Program.Client.GetInteractivity();
            var pollTime = TimeSpan.FromSeconds(10);

            DiscordEmoji[] emojiOptions = { DiscordEmoji.FromName(Program.Client, ":one:"),
                                            DiscordEmoji.FromName(Program.Client, ":two:"),
                                            DiscordEmoji.FromName(Program.Client, ":three:"),
                                            DiscordEmoji.FromName(Program.Client, ":four:") };

            string optionsDescription = $"{emojiOptions[0]} | {option1} \n" +
                                        $"{emojiOptions[1]} | {option2} \n" +
                                        $"{emojiOptions[2]} | {option3} \n" +
                                        $"{emojiOptions[3]} | {option4}";

            var pollMessage = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Red,
                Title = pollTitle,
                Description = optionsDescription
            };

            var sentPoll = await ctx.Channel.SendMessageAsync(embed: pollMessage);
            foreach (var emoji in emojiOptions)
            {
                await sentPoll.CreateReactionAsync(emoji);
            }

            var totalReactions = await interactivity.CollectReactionsAsync(sentPoll, pollTime);

            int count1 = 0;
            int count2 = 0;
            int count3 = 0;
            int count4 = 0;

            foreach (var emoji in totalReactions)
            {
                if (emoji.Emoji == emojiOptions[0])
                {
                    count1++;
                }
                if (emoji.Emoji == emojiOptions[1])
                {
                    count2++;
                }
                if (emoji.Emoji == emojiOptions[2])
                {
                    count3++;
                }
                if (emoji.Emoji == emojiOptions[3])
                {
                    count4++;
                }
            }

            int totalVotes = count1 + count2 + count3 + count4;
            string resultsDescription = $"{emojiOptions[0]}: {count1} Votes \n" +
                                        $"{emojiOptions[1]}: {count2} Votes \n" +
                                        $"{emojiOptions[2]}: {count3} Votes \n" +
                                        $"{emojiOptions[3]}: {count4} Votes \n\n" +
                                        $"Total Votes: {totalVotes}";

            var resultEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Green,
                Title = "Results of the Poll",
                Description = resultsDescription
            };

            await ctx.Channel.SendMessageAsync(embed: resultEmbed);
        }
    }
}
