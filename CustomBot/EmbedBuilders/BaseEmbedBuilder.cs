using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomBot.EmbedBuilders
{
    public abstract class BaseEmbedBuilder
    {
        public DiscordEmbedBuilder EmbedBuilder { get; protected set; } = new DiscordEmbedBuilder { Color = DiscordColor };

        public static DiscordColor DiscordColor { get; protected set; } = new DiscordColor(50, 233, 228);

    }
}
