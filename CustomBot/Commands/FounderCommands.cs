using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CustomBot.Commands
{
    [Group("founder")]
    public class FounderCommands : BaseCommandModule
    {
        [Command("sourcecode")]
        public async Task SourceCode(CommandContext ctx)
        {
            var github = new Uri("https://github.com/karolNiedziela/DiscordBot");
            await ctx.Channel.SendMessageAsync($"{github.AbsoluteUri}");
        }
    }
}
