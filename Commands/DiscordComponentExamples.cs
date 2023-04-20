using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeTestBot.Commands
{
    public class DiscordComponentExamples : BaseCommandModule
    {
        [Command("button")]
        public async Task Buttons(CommandContext ctx)
        {
            DiscordButtonComponent button1 = new DiscordButtonComponent(ButtonStyle.Primary, "1", "Button 1");
            DiscordButtonComponent button2 = new DiscordButtonComponent(ButtonStyle.Primary, "2", "Button 2");

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Azure)
                    .WithTitle("This is a embed message with Buttons")
                    .WithDescription("Click on a button to see its functionality!!"))
                .AddComponents(button1, button2);

            await ctx.Channel.SendMessageAsync(message);
        }

        [Command("list")]
        public async Task DropDownList(CommandContext ctx)
        {
            List<DiscordSelectComponentOption> selectOptions = new List<DiscordSelectComponentOption>();
            selectOptions.Add(new DiscordSelectComponentOption("Option 1", "option1"));
            selectOptions.Add(new DiscordSelectComponentOption("Option 2", "option2"));
            selectOptions.Add(new DiscordSelectComponentOption("Option 3", "option3"));

            IEnumerable<DiscordSelectComponentOption> options = selectOptions.AsEnumerable();

            var dropDown = new DiscordSelectComponent("dropDown1", "Click me to view options", options);

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Azure)
                    .WithTitle("This embed contains a drop-down list")
                    .WithDescription("Click on the drop-down to see how it works"))
                .AddComponents(dropDown);

            await ctx.Channel.SendMessageAsync(message);
        }
    }
}
