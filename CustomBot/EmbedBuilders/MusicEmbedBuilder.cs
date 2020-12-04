using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomBot.EmbedBuilders
{
    public class MusicEmbedBuilder : BaseEmbedBuilder
    {
        public MusicEmbedBuilder(string description)
        {
            EmbedBuilder.Title = nameof(MusicEmbedBuilder).Replace("EmbedBuilder", string.Empty);
            EmbedBuilder.Description = description;
        }
    }
}
