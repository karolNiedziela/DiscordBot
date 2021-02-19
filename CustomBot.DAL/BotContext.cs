using CustomBot.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomBot.DAL
{
    public class BotContext : DbContext
    {

        public BotContext(DbContextOptions<BotContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlaylistSong>().HasKey(ps => new { ps.PlaylistId, ps.SongId });
        }

        public DbSet<Song> Songs { get; set; }

        public DbSet<Playlist> Playlists { get; set; }

        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
    }
}
