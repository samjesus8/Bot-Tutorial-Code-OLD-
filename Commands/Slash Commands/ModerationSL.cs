using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Threading.Tasks;

namespace YouTubeTestBot.Commands.Slash_Commands
{
    public class ModerationSL : ApplicationCommandModule
    {
        [SlashCommand("ban", "Bans a user from the server")]
        public async Task Ban(InteractionContext ctx, [Option("user", "The user you want to ban")] DiscordUser user,
                                                      [Option("reason", "Reason for ban")] string reason = null) 
        {
            await ctx.DeferAsync();

            if (ctx.Member.Permissions.HasPermission(Permissions.Administrator)) 
            {
                var member = (DiscordMember)user;
                await ctx.Guild.BanMemberAsync(member, 0, reason);

                var banMessage = new DiscordEmbedBuilder()
                {
                    Title = "Banned user " + member.Username,
                    Description = "Reason: " + reason,
                    Color = DiscordColor.Red
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(banMessage));
            }
            else 
            {
                var nonAdminMessage = new DiscordEmbedBuilder()
                {
                    Title = "Access Denied",
                    Description = "You need to be an Administrator to execute this command",
                    Color = DiscordColor.Red
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(nonAdminMessage));
            }
        }

        [SlashCommand("kick", "Kick a user from the server")]
        public async Task Kick(InteractionContext ctx, [Option("user", "The user you want to kick")] DiscordUser user) 
        {
            await ctx.DeferAsync();

            if (ctx.Member.Permissions.HasPermission(Permissions.Administrator)) 
            {
                var member = (DiscordMember)user;
                await member.RemoveAsync();

                var kickMessage = new DiscordEmbedBuilder()
                {
                    Title = member.Username + " got kicked from the Server",
                    Description = "Kicked by " + ctx.User.Username,
                    Color = DiscordColor.Red
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(kickMessage));
            }
            else 
            {
                var nonAdminMessage = new DiscordEmbedBuilder()
                {
                    Title = "Access Denied",
                    Description = "You need to be an Administrator to execute this command",
                    Color = DiscordColor.Red
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(nonAdminMessage));
            }
        }

        [SlashCommand("timeout", "Timeout a user")]
        public async Task Timeout(InteractionContext ctx, [Option("user", "The user you want to timeout")] DiscordUser user,
                                                          [Option("duration", "Duration of the timeout in seconds")] long duration)
        {
            await ctx.DeferAsync();

            if (ctx.Member.Permissions.HasPermission(Permissions.Administrator)) 
            {
                var timeDuration = DateTime.Now + TimeSpan.FromSeconds(duration);
                var member = (DiscordMember)user;
                await member.TimeoutAsync(timeDuration);

                var timeoutMessage = new DiscordEmbedBuilder()
                {
                    Title = member.Username + "has been timeout",
                    Description = "Duration: " + TimeSpan.FromSeconds(duration).ToString(),
                    Color = DiscordColor.Red
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(timeoutMessage));
            }
            else 
            {
                var nonAdminMessage = new DiscordEmbedBuilder()
                {
                    Title = "Access Denied",
                    Description = "You need to be an Administrator to execute this command",
                    Color = DiscordColor.Red
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(nonAdminMessage));
            }
        }
    }
}
