using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;

namespace YouTubeTestBot.Commands
{
    public class FunCommands : BaseCommandModule
    {
        [Command("test")]
        public async Task TestCommand(CommandContext ctx) 
        {
            Console.WriteLine("working");
            await ctx.Channel.SendMessageAsync("Hello");
        }

        [Command("add")]
        public async Task Addition(CommandContext ctx, int number1, int number2) 
        {
            int answer = number1 + number2;
            await ctx.Channel.SendMessageAsync(answer.ToString());
        }

        [Command("subtract")]
        public async Task Subtract(CommandContext ctx, int number1, int number2)
        {
            int answer = number1 - number2;
            await ctx.Channel.SendMessageAsync(answer.ToString());
        }
    }
}
