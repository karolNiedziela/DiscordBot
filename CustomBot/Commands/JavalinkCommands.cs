using CustomBot.EmbedBuilders;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomBot.Commands
{
    public class JavalinkCommands : BaseCommandModule
    {
        [Command("join")]
        [Description("Join to voice channel")]
        public async Task Join(CommandContext ctx, DiscordChannel channel = null)
        {
            var lava = ctx.Client.GetLavalink();

            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("The Lavalink connection is not established.");
                return;
            }

            var node = lava.ConnectedNodes.Values.First();


            var vstat = ctx.Member?.VoiceState;
            if (vstat?.Channel == null && channel == null)
            {
                // they did not specify a channel and are not in one
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }
            if (channel == null)
                channel = vstat.Channel;

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
                return;
            }

            await node.ConnectAsync(channel);

            var embed = new MusicEmbedBuilder($"Joining channel {Formatter.InlineCode(channel.Name)}");

            await ctx.RespondAsync(embed: embed.EmbedBuilder);
        }

        [Command("leave")]
        [Description("Leave voice channel")]
        public async Task Leave(CommandContext ctx, DiscordChannel channel = null)
        {
            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("The Lavalink connection is not established");
                return;
            }

            var node = lava.ConnectedNodes.Values.First();

            var vstat = ctx.Member?.VoiceState;

            if (channel == null)
                channel = vstat.Channel;

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
            }

            var connection = node.GetGuildConnection(channel.Guild);

            if (connection == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
            }

            await connection.DisconnectAsync();

            var embed = new MusicEmbedBuilder($"Leaving channel {Formatter.InlineCode(channel.Name)}");

            await ctx.RespondAsync(embed: embed.EmbedBuilder);
        }

        [Command("play")]
        [Description("Play first song which was found on youtube by search")]
        public async Task Play(CommandContext ctx, [Description("Artist and song title")][RemainingText] string search)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var connection = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (connection == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            var loadResult = await node.Rest.GetTracksAsync(search, LavalinkSearchType.Youtube);

            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
                || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.RespondAsync($"Track search failed for {search}.");
                return;
            }

            var track = loadResult.Tracks.First();

            await connection.PlayAsync(track);

            var embed = new MusicEmbedBuilder($"Now playing {Formatter.MaskedUrl(track.Title, track.Uri)} requested by {Formatter.Mention(ctx.User, true)}");

            await ctx.RespondAsync(embed: embed.EmbedBuilder);
        }

        [Command("pause")]
        [Description("Pause current song")]
        public async Task Pause(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var connection = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (connection == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            if (connection.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("There are not track loaded.");
                return;
            }

            await connection.PauseAsync();

            var embed = new MusicEmbedBuilder($"Pause on demand by {Formatter.Mention(ctx.User, true)}");

            await ctx.RespondAsync(embed: embed.EmbedBuilder);
        }

        [Command("resume")]
        [Description("Resume song which was paused")]
        public async Task Resume(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var connection = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (connection == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            if (connection.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("There are not track loaded.");
                return;
            }

            await connection.ResumeAsync();

            var embed = new MusicEmbedBuilder($"Resume on demand by {Formatter.Mention(ctx.User, true)}");

            await ctx.RespondAsync(embed: embed.EmbedBuilder);
        }

        [Command("stop")]
        [Description("Stop music player")]
        public async Task Stop(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var connection = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (connection == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            if (connection.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("There are not track loaded.");
                return;
            }

            await connection.StopAsync();

            var embed = new MusicEmbedBuilder($"Playback stopped completely by {Formatter.Mention(ctx.User, true)}");

            await ctx.RespondAsync(embed: embed.EmbedBuilder);
        }

        [Command("setVolume")]
        [Description("Set volume")]
        public async Task SetVolume(CommandContext ctx, [Description("Minimum is 0, Maximum is 100")] int volume)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            if (volume > 100 || volume < 0)
            {
                await ctx.RespondAsync($"Volume cannot be {volume}. Must be between 0 and 100.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var connection = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (connection == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            if (connection.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("There are not track loaded.");
                return;
            }

            await connection.SetVolumeAsync(volume);

            var embed = new MusicEmbedBuilder($"Volume changed. Current volume is {volume} by {Formatter.Mention(ctx.User, true)}");

            await ctx.RespondAsync(embed: embed.EmbedBuilder);
        }
    }
}
