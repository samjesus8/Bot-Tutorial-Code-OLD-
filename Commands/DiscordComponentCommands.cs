using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeTestBot.Commands
{
    public class DiscordComponentCommands : BaseCommandModule
    {
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

        [Command("dropdown-list")]
        public async Task DropdownList(CommandContext ctx)
        {
            List<DiscordSelectComponentOption> optionList = new List<DiscordSelectComponentOption>();
            optionList.Add(new DiscordSelectComponentOption("Option 1", "option1"));
            optionList.Add(new DiscordSelectComponentOption("Option 2", "option2"));
            optionList.Add(new DiscordSelectComponentOption("Option 3", "option3"));

            var options = optionList.AsEnumerable();

            var dropDown = new DiscordSelectComponent("dropDownList", "Select...", options);

            var dropDownMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Gold)
                    .WithTitle("This embed has a drop-down list on it"))
                .AddComponents(dropDown);

            await ctx.Channel.SendMessageAsync(dropDownMessage);
        }

        [Command("channel-list")]
        public async Task ChannelList(CommandContext ctx)
        {
            var channelComponent = new DiscordChannelSelectComponent("channelDropDownList", "Select...");

            var dropDownMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Gold)
                    .WithTitle("This embed has a channel drop-down list on it"))
                .AddComponents(channelComponent);

            await ctx.Channel.SendMessageAsync(dropDownMessage);
        }

        [Command("mention-list")]
        public async Task MentionList(CommandContext ctx)
        {
            var mentionComponent = new DiscordMentionableSelectComponent("mentionDropDownList", "Select...");

            var dropDownMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Gold)
                    .WithTitle("This embed has a mention drop-down list on it"))
                .AddComponents(mentionComponent);

            await ctx.Channel.SendMessageAsync(dropDownMessage);
        }
    }
}
