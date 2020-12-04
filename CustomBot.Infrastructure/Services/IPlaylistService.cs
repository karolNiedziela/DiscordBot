using CustomBot.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CustomBot.Infrastructure.Services
{
    public interface IPlaylistService
    {

        Task<Playlist> GetAsync(string playlistName);

        Task AddPlaylist(Playlist playlist); 

        Task AddSongToPlaylistAsync(string songName, Playlist playlist);

        Task DeleteSongFromPlaylistAsync(string songName, Playlist playlist);


    }
}
