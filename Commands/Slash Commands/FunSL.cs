using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System;
using System.Threading.Tasks;

namespace YouTubeTestBot.Commands.Slash_Commands
{
    public class FunSL : ApplicationCommandModule
    {
        [SlashCommand("test", "This is our first Slash Command")]
        public async Task TestSlashCommand(InteractionContext ctx, [Choice("Pre-Defined Text", "afhajfjafjdghldghlhg")]
                                                                   [Option("string", "Type in anything you want")] string text)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                                                            .WithContent("Starting Slash Command...."));

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = text,
            };

            await ctx.Channel.SendMessageAsync(embed: embedMessage);
        }

        [SlashCommand("poll", "Create your own poll")]
        public async Task PollCommand(InteractionContext ctx, [Option("question", "The main poll subject/question")] string Question,
                                                              [Option("timelimit", "The time set on this poll")] long TimeLimit,
                                                              [Option("option1", "Option 1")] string Option1,
                                                              [Option("option2", "Option 1")] string Option2,
                                                              [Option("option3", "Option 1")] string Option3,
                                                              [Option("option4", "Option 1")] string Option4)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                                                .WithContent("..."));

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

        [SlashCommand("caption", "Give any image a Caption")]
        public async Task CaptionCommand(InteractionContext ctx, [Option("caption", "The caption you want the image to have")] string caption,
                                                                 [Option("image", "The image you want to upload")] DiscordAttachment picture) 
        {
            await ctx.DeferAsync();

            var captionMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Azure)
                    .WithFooter(caption)
                    .WithImageUrl(picture.Url)
                    );

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(captionMessage.Embed));
        }

        //User Requested Commands

        [SlashCommand("create-VC", "Creates a voice channel")]
        public async Task CreateVC(InteractionContext ctx, [Option("channel-name", "Name of this Voice Channel")] string channelName,
                                                           [Option("member-limit", "Adds a user limit to this channel")] string channelLimit = null)
        {
            await ctx.DeferAsync();

            var channelUsersParse = int.TryParse(channelLimit, out int channelUsers);

            //Create the Voice Channel with the channel limit
            if (channelLimit != null && channelUsersParse == true)
            {
                await ctx.Guild.CreateVoiceChannelAsync(channelName, null, null, channelUsers);

                var success = new DiscordEmbedBuilder()
                {
                    Title = "Created Voice Channel " + channelName,
                    Description = "The channel was created with a user limit of " + channelLimit.ToString(),
                    Color = DiscordColor.Azure
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(success));
            }
            //No User Limit
            else if (channelLimit == null)
            {
                await ctx.Guild.CreateVoiceChannelAsync(channelName);

                var success = new DiscordEmbedBuilder()
                {
                    Title = "Created Voice Channel " + channelName,
                    Color = DiscordColor.Azure
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(success));
            }
            //Invalid input parsed in
            else if (channelUsersParse == false)
            {
                var fail = new DiscordEmbedBuilder()
                {
                    Title = "Please provide a valid number for the user limit",
                    Color = DiscordColor.Red
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(fail));
            }
        }

        [SlashCommand("modal", "Show a modal")]
        public async Task Modal(InteractionContext ctx)
        {
            var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("Test Modal")
                .WithCustomId("modal")
                .AddComponents(new TextInputComponent("Random", "randomTextBox", "Type something here"));

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, modal);
        }
    }
}
