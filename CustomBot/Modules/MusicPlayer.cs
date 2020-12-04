using CustomBot.DAL;
using CustomBot.DAL.Models;
using CustomBot.EmbedBuilders;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DSharpPlus.Entities.DiscordEmbedBuilder;

namespace CustomBot.Modules
{
    public class MusicPlayer
    {
        private readonly CommandContext _ctx;

        private readonly BotContext _context;

        public Queue<LavalinkTrack> Queue { get; set; }

        public LavalinkTrack CurrentSong { get; set; }

        public Playlist Playlist { get; set; }

        public List<LavalinkTrack> Tracks { get; set; }

        public bool IsSongPlaying { get; set; }

        public bool IsSongSkipped { get; set; } = false;

        public LavalinkExtension Lava { get; set; }

        public LavalinkNodeConnection Node { get; set; }

        public LavalinkGuildConnection Connection { get; set; }

        public MusicPlayer(CommandContext ctx, BotContext context)
        {
            _ctx = ctx;
            Queue = new Queue<LavalinkTrack>();
            _context = context;
            Tracks = new List<LavalinkTrack>();
            Lava = _ctx.Client.GetLavalink();
            Node = Lava.ConnectedNodes.Values.First();
            Connection = Node.GetGuildConnection(_ctx.Member.VoiceState.Guild);
        }

        public async Task AddSong(Song song)
        {
            var loadResult = await Node.Rest.GetTracksAsync(song.Title, LavalinkSearchType.Youtube);
            Queue.Enqueue(loadResult.Tracks.First());
            GetCurrentTrackAsync();
        }

        public void RemoveSong()
        {
            Queue.Dequeue();
        }

        private void GetCurrentTrackAsync()
        {
            CurrentSong = Queue.Peek();
        }

        public async Task GetQueue()
        {
            if (_ctx.Member.VoiceState == null || _ctx.Member.VoiceState.Channel == null)
            {
                await _ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            if (Connection == null)
            {
                await _ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            if (Playlist == null)
            {
                await _ctx.RespondAsync("No playlist loaded.");
                return;
            }

            var interactivity = _ctx.Client.GetInteractivity();
            
            var pageCount = Queue.Count / 5 + 1;
            if (Queue.Count % 5 == 0) 
                pageCount--;    

            var pages = Queue.Select(x => x.Title)
                 .Select((t, i) => new { title = t, index = i })
                 .GroupBy(x => x.index / 5)
                 .Select(page => new Page("", 
                 new DiscordEmbedBuilder
                 {
                     Title = "Playlist",
                     Description = $"Now playing: {Queue.Peek().Title}\n\n{string.Join("\n", page.Select(track => $"`{track.index + 1:00}` {track.title}"))}",
                     Footer = new EmbedFooter
                     {
                         Text = $"Page {page.Key + 1}/{pageCount}"
                     }
                 }
                 )).ToArray();

            var emojis = new PaginationEmojis
            {
                SkipLeft = null,
                SkipRight = null,
                Stop = null,
                Left = DiscordEmoji.FromUnicode("◀"),
                Right = DiscordEmoji.FromUnicode("▶")
            };

            await interactivity.SendPaginatedMessageAsync(_ctx.Channel, _ctx.User, pages, emojis, PaginationBehaviour.Ignore, PaginationDeletion.KeepEmojis, TimeSpan.FromMinutes(2));
        }

        public async Task Play()
        {
            if (_ctx.Member.VoiceState == null || _ctx.Member.VoiceState.Channel == null)
            {
                await _ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            if (Connection == null)
            {
                await _ctx.RespondAsync("Lavalink is not connected.");
                return;
            }
                          
            await Connection.PlayAsync(CurrentSong);
            
            var embed = new PlaylistEmbedBuilder($"Now playing { Formatter.MaskedUrl(Queue.Peek().Title, Queue.Peek().Uri) }");

            await _ctx.RespondAsync(embed: embed.EmbedBuilder);   
        }

        public async Task Skip()
        {
            await Connection.StopAsync();

            RemoveSong();

            GetCurrentTrackAsync();

            await Play();
        }
    }
}
