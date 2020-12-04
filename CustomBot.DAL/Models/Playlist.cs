using System;
using System.Collections.Generic;
using System.Text;

namespace CustomBot.DAL.Models
{
    public class Playlist : BaseEntity
    {
        public string Name { get; set; }

        public string ServerName { get; set; }

        public ICollection<PlaylistSong> Songs { get; set; } = new List<PlaylistSong>();
    }
}
