using System;
using System.Collections.Generic;
using System.Text;

namespace CustomBot.DAL.Models
{
    public class Song : BaseEntity
    {
        public string Title { get; set; }

        public ICollection<PlaylistSong> Playlists { get; set; } = new List<PlaylistSong>();

    }
}
