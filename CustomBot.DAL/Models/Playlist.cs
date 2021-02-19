using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CustomBot.DAL.Models
{
    public class Playlist : BaseEntity
    {
        public string Name { get; set; }

        public ulong ServerId { get; set; }

        public string ServerName { get; set; }

        public ICollection<PlaylistSong> Songs { get; set; } = new List<PlaylistSong>();
    }
}
