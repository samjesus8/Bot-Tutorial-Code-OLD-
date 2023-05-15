using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeTestBot.Commands.Prefix
{
    public class MusicCommands : BaseCommandModule
    {
        [Command("play")]
        public async Task PlayMusic(CommandContext ctx, [RemainingText] string query)
        {
            var userVC = ctx.Member.VoiceState.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //PRE-EXECUTION CHECKS
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.Channel.SendMessageAsync("Please enter a VC!!!");
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.Channel.SendMessageAsync("Connection is not Established!!!");
                return;
            }

            if (userVC.Type != ChannelType.Voice)
            {
                await ctx.Channel.SendMessageAsync("Please enter a valid VC!!!");
                return;
            }

            //Connecting to the VC and playing music
            var node = lavalinkInstance.ConnectedNodes.Values.First();
            await node.ConnectAsync(userVC);

            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            if (conn == null)
            {
                await ctx.Channel.SendMessageAsync("Lavalink Failed to connect!!!");
                return;
            }

            var searchQuery = await node.Rest.GetTracksAsync(query);
            if (searchQuery.LoadResultType == LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType == LavalinkLoadResultType.LoadFailed)
            {
                await ctx.Channel.SendMessageAsync($"Failed to find music with query: {query}");
                return;
            }

            var musicTrack = searchQuery.Tracks.First();

            await conn.PlayAsync(musicTrack);

            string musicDescription = $"Now Playing: {musicTrack.Title} \n" +
                                      $"Author: {musicTrack.Author} \n" +
                                      $"URL: {musicTrack.Uri}";

            var nowPlayingEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Purple,
                Title = $"Successfully joined channel {userVC.Name} and playing music",
                Description = musicDescription
            };

            await ctx.Channel.SendMessageAsync(embed: nowPlayingEmbed);
        }

        [Command("pause")]
        public async Task PauseMusic(CommandContext ctx)
        {
            var userVC = ctx.Member.VoiceState.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //PRE-EXECUTION CHECKS
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.Channel.SendMessageAsync("Please enter a VC!!!");
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.Channel.SendMessageAsync("Connection is not Established!!!");
                return;
            }

            if (userVC.Type != ChannelType.Voice)
            {
                await ctx.Channel.SendMessageAsync("Please enter a valid VC!!!");
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.Channel.SendMessageAsync("Lavalink Failed to connect!!!");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.Channel.SendMessageAsync("No tracks are playing!!!");
                return;
            }

            await conn.PauseAsync();

            var pausedEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Yellow,
                Title = "Track Paused!!"
            };

            await ctx.Channel.SendMessageAsync(embed: pausedEmbed);
        }

        [Command("resume")]
        public async Task ResumeMusic(CommandContext ctx)
        {
            var userVC = ctx.Member.VoiceState.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //PRE-EXECUTION CHECKS
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.Channel.SendMessageAsync("Please enter a VC!!!");
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.Channel.SendMessageAsync("Connection is not Established!!!");
                return;
            }

            if (userVC.Type != ChannelType.Voice)
            {
                await ctx.Channel.SendMessageAsync("Please enter a valid VC!!!");
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.Channel.SendMessageAsync("Lavalink Failed to connect!!!");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.Channel.SendMessageAsync("No tracks are playing!!!");
                return;
            }

            await conn.ResumeAsync();

            var resumedEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green,
                Title = "Resumed"
            };

            await ctx.Channel.SendMessageAsync(embed: resumedEmbed);
        }

        [Command("stop")]
        public async Task StopMusic(CommandContext ctx)
        {
            var userVC = ctx.Member.VoiceState.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //PRE-EXECUTION CHECKS
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.Channel.SendMessageAsync("Please enter a VC!!!");
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.Channel.SendMessageAsync("Connection is not Established!!!");
                return;
            }

            if (userVC.Type != ChannelType.Voice)
            {
                await ctx.Channel.SendMessageAsync("Please enter a valid VC!!!");
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.Channel.SendMessageAsync("Lavalink Failed to connect!!!");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.Channel.SendMessageAsync("No tracks are playing!!!");
                return;
            }

            await conn.StopAsync();
            await conn.DisconnectAsync();

            var stopEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Red,
                Title = "Stopped the Track",
                Description = "Successfully disconnected from the VC"
            };

            await ctx.Channel.SendMessageAsync(embed: stopEmbed);
        }
    }
}
