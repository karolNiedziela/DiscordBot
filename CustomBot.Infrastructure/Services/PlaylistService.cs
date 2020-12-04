using CustomBot.DAL;
using CustomBot.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBot.Infrastructure.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly BotContext _context;

        public PlaylistService(BotContext context)
        {
            _context = context;
        }

        public async Task<Playlist> GetAsync(string playlistName)
        {
            var playlist = await _context.Playlists.SingleOrDefaultAsync(p => p.Name == playlistName);

            if (playlist == null)
            {
                throw new Exception("Playlist not found");
            }

            return playlist;
        }

        public async Task AddPlaylist(Playlist playlist)
        {
            await _context.Playlists.AddAsync(playlist);
            await _context.SaveChangesAsync();
        }

        public async Task AddSongToPlaylistAsync(string songName, Playlist playlist)
        {
            var song = playlist.Songs.Select(s => s.Song).SingleOrDefault(s => s.Title == songName);

            if (song != null)
            {
                throw new Exception("Song is already in playlist.");
            }

            playlist.Songs.Add(new PlaylistSong
            {
                Playlist = playlist,
                Song = new Song { Title = songName }
            });

            await _context.SaveChangesAsync();
        }


        public async Task DeleteSongFromPlaylistAsync(string songName, Playlist playlist)
        {
            var song = await _context.Songs.FirstOrDefaultAsync(s => s.Title == songName);

            var playlistSong = playlist.Songs.FirstOrDefault(s => s.SongId == song.Id);

            if (song == null)
            {
                throw new Exception("Song is not in playlist.");
            }

            playlist.Songs.Remove(playlistSong);

            await _context.SaveChangesAsync();
        }
        
    }
}
