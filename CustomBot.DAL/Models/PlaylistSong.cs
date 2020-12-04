using System;
using System.Collections.Generic;
using System.Text;

namespace CustomBot.DAL.Models
{
    public class PlaylistSong
    {
        public int PlaylistId { get; set; }

        public Playlist Playlist { get; set; }

        public int SongId { get; set; }

        public Song Song { get; set; }
    }
}
