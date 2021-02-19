using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CustomBot.DAL;
using CustomBot.Infrastructure.Services;


namespace CustomBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddDbContext<BotContext>(options =>
            {
                options.UseSqlServer("Server=DESKTOP-Q4HKDE5\\SQLEXPRESS;Database=CustomBot;Trusted_Connection=True;MultipleActiveResultSets=true",
                    x => x.MigrationsAssembly("CustomBot.DAL.Migrations"));
            });

            services.AddScoped<IPlaylistService, PlaylistService>();

            var serviceProvider = services.BuildServiceProvider();

            var bot = new Bot(serviceProvider);
            bot.RunAsync().GetAwaiter().GetResult();

        }
    }
}