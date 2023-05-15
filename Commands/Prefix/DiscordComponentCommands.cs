using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeTestBot.Commands.Prefix
{
    public class DiscordComponentCommands : BaseCommandModule
    {
        //This class shows how to make implement different DiscordComponent types

        [Command("button")]
        public async Task ButtonExample(CommandContext ctx)
        {
            //Declare your buttons before doing anything else

            DiscordButtonComponent button1 = new DiscordButtonComponent(ButtonStyle.Primary, "1", "Button 1");
            DiscordButtonComponent button2 = new DiscordButtonComponent(ButtonStyle.Primary, "2", "Button 2");

            //Make the MessageBuilder and add on the buttons
            //A Message can have up to 5 x 5 worth of buttons. Thats 5 rows, each with 5 buttons
            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Azure)
                    .WithTitle("This is a message with buttons")
                    .WithDescription("Please select a button"))
                .AddComponents(button1, button2);

            await ctx.Channel.SendMessageAsync(message);
        }

        [Command("dropdown-list")]
        public async Task DropdownList(CommandContext ctx)
        {
            //Declare the list of options in the drop-down
            List<DiscordSelectComponentOption> optionList = new List<DiscordSelectComponentOption>();
            optionList.Add(new DiscordSelectComponentOption("Option 1", "option1"));
            optionList.Add(new DiscordSelectComponentOption("Option 2", "option2"));
            optionList.Add(new DiscordSelectComponentOption("Option 3", "option3"));

            //Turn the list into an IEnumerable for the Component
            var options = optionList.AsEnumerable();

            //Make the drop-down component
            var dropDown = new DiscordSelectComponent("dropDownList", "Select...", options);

            //Make and send off the message with the component
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
