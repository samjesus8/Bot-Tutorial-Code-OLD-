using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
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

        [Command("embedmessage1")]
        public async Task SendEmbedMessage2(CommandContext ctx) //Example 1
        {
            var embedMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                .WithColor(DiscordColor.Azure)
                .WithTitle("This is a title")
                .WithDescription("This is a description")
                );

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

        [Command("poll")]
        public async Task PollCommand(CommandContext ctx, int TimeLimit, string Option1, string Option2, string Option3, string Option4, params string[] Question) 
        {
            var interactvity = ctx.Client.GetInteractivity(); //Getting the Interactivity Module
            TimeSpan timer = TimeSpan.FromSeconds(TimeLimit); //Converting my time parameter to a timespan variable

            DiscordEmoji[] optionEmojis = { DiscordEmoji.FromName(ctx.Client, ":one:", false),
                                            DiscordEmoji.FromName(ctx.Client, ":two:", false),
                                            DiscordEmoji.FromName(ctx.Client, ":three:", false),
                                            DiscordEmoji.FromName(ctx.Client, ":four:", false) }; //Array to store discord emojis

            string optionsString = optionEmojis[0] + " | " + Option1 + "\n" +
                                   optionEmojis[1] + " | " + Option2 + "\n" +
                                   optionEmojis[2] + " | " + Option3 + "\n" +
                                   optionEmojis[3] + " | " + Option4; //String to display each option with its associated emojis

            var pollMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                .WithColor(DiscordColor.Azure)
                .WithTitle(string.Join(" ", Question))
                .WithDescription(optionsString)
                ); //Making the Poll message

            var putReactOn = await ctx.Channel.SendMessageAsync(pollMessage); //Storing the await command in a variable

            foreach (var emoji in optionEmojis) 
            {
                await putReactOn.CreateReactionAsync(emoji); //Adding each emoji from the array as a reaction on this message
            }

            var result = await interactvity.CollectReactionsAsync(putReactOn, timer); //Collects all the emoji's and how many peopele reacted to those emojis

            int count1 = 0; //Counts for each emoji
            int count2 = 0;
            int count3 = 0;
            int count4 = 0;

            foreach (var emoji in result) //Foreach loop to go through all the emojis in the message and filtering out the 4 emojis we need
            {
                if (emoji.Emoji == optionEmojis[0]) 
                {
                    count1++;
                }
                if (emoji.Emoji == optionEmojis[1]) 
                {
                    count2++;
                }
                if (emoji.Emoji == optionEmojis[2]) 
                {
                    count3++;
                }
                if (emoji.Emoji == optionEmojis[3]) 
                {
                    count4++;
                }
            }

            int totalVotes = count1 + count2 + count3 + count4;

            string resultsString = optionEmojis[0] + ": " + count1 + " Votes \n" +
                       optionEmojis[1] + ": " + count2 + " Votes \n" +
                       optionEmojis[2] + ": " + count3 + " Votes \n" +
                       optionEmojis[3] + ": " + count4 + " Votes \n\n" +
                       "The total number of votes is " + totalVotes; //String to show the results of the poll

            var resultsMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                .WithColor(DiscordColor.Green)
                .WithTitle("Results of Poll")
                .WithDescription(resultsString)
                );

            await ctx.Channel.SendMessageAsync(resultsMessage); //Making the embed and sending it off
        }
    }
}
