using CustomBot.DAL;
using CustomBot.DAL.Models;
using CustomBot.EmbedBuilders;
using CustomBot.Infrastructure.Services;
using CustomBot.Modules;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBot.Commands
{
    [Group("playlist")]
    [Description("Management of playlists")]
    public class PlaylistCommands : BaseCommandModule
    {
        private readonly BotContext _context;
        private readonly IPlaylistService _playlistService;

        public MusicPlayer MusicPlayer { get; set; }

        public PlaylistCommands(BotContext context, IPlaylistService playlistService)
        {
            _context = context;
            _playlistService = playlistService;
        }

        [Command("create")]
        [Description("Create own playlist saved to current guild")]
        public async Task Create(CommandContext ctx, [Description("Your playlist name")] string playlistName)
        {
            var playlist = await _context.Playlists.SingleOrDefaultAsync(p => p.Name == playlistName && p.ServerId == ctx.Guild.Id);

            if (playlist != null)
            {
                await ctx.RespondAsync($"Playlist {Formatter.Bold(playlistName)} already exists on this server.");
                return;
            }

            playlist = new Playlist
            {
                Name = playlistName,
                ServerId = ctx.Guild.Id,
                ServerName = ctx.Guild.Name
            };

            await _playlistService.AddPlaylist(playlist);

            var embed = new PlaylistEmbedBuilder($"New playlist {Formatter.Bold(playlistName)} created successfully by {Formatter.Mention(ctx.User, true)}.");

            await ctx.RespondAsync(embed: embed.EmbedBuilder);
        }

        [Command("add")]
        [Description("Add song to your playlist")]
        public async Task Add(CommandContext ctx, [Description("Playlist name")] string playlistName, [Description("Song name")][RemainingText] string songName)
        {
            var playlist = await _context.Playlists.SingleOrDefaultAsync(p => p.Name == playlistName && p.ServerId == ctx.Guild.Id);

            if (playlist == null)
            {
                await ctx.RespondAsync($"Playlist {Formatter.Bold(playlistName)} does not exist.");
                return;
            }

            try
            {
                await _playlistService.AddSongToPlaylistAsync(songName, playlist);
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
                return;
            }

            var embed = new PlaylistEmbedBuilder($"Song {Formatter.Bold(songName)} added successfully to playlist {Formatter.Bold(playlistName)}");

            await ctx.RespondAsync(embed: embed.EmbedBuilder);

        }

        [Command("delete")]
        [Description("Delete song from your playlist")]
        public async Task Delete(CommandContext ctx, [Description("Playlist name")] string playlistName, [Description("Song name")][RemainingText] string songName)
        {
            var playlist = await _context.Playlists.Include(p => p.Songs).SingleOrDefaultAsync(p => p.Name == playlistName && p.ServerId == ctx.Guild.Id);

            if (playlist == null)
            {
                await ctx.RespondAsync($"Playlist {Formatter.Bold(playlistName)} does not exist.");
                return;
            }

            try
            {
                await _playlistService.DeleteSongFromPlaylistAsync(songName, playlist);
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
                return;
            }

            var embed = new PlaylistEmbedBuilder($"Song {Formatter.Bold(songName)} deleted successfully from playlist {Formatter.Bold(playlistName)}");

            await ctx.RespondAsync(embed: embed.EmbedBuilder);
        }

        [Command("load")]
        [Description("Start playlist")]
        public async Task Load(CommandContext ctx, [Description("Playlist name")] string playlistName)
        {
            var playlist = await _context.Playlists.Include(p => p.Songs).ThenInclude(s => s.Song)
                                 .SingleOrDefaultAsync(p => p.Name == playlistName && p.ServerId == ctx.Guild.Id);

            if (playlist == null)
            {
                await ctx.RespondAsync($"Playlist {Formatter.Bold(playlistName)} does not exist.");
                return;
            }

            var songs = playlist.Songs.Select(s => s.Song).ToList();

            MusicPlayer = new MusicPlayer(ctx, _context)
            {
                Playlist = playlist
            };

            foreach (var song in songs)
            {
                await MusicPlayer.AddSong(song);
            }
            await MusicPlayer.Play();
        }

        [Command("queue")]
        [Description("Display playlist queue")]
        public async Task Queue(CommandContext ctx)
        {
            await MusicPlayer.GetQueue();
        }

        [Command("skip")]
        [Description("Skip current song")]
        public async Task Skip(CommandContext ctx)
        {
            await ctx.RespondAsync($"Song skipped requested by by {Formatter.Mention(ctx.User, true)}.");

            await MusicPlayer.Skip();
        }
    }
}
