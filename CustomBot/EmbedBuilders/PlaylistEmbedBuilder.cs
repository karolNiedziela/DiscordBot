using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomBot.EmbedBuilders
{
    public class PlaylistEmbedBuilder : BaseEmbedBuilder
    {
        public PlaylistEmbedBuilder(string description)
        {
            EmbedBuilder.Title = nameof(PlaylistEmbedBuilder).Replace("EmbedBuilder", string.Empty);
            EmbedBuilder.Description = description;
        }
    }
}
