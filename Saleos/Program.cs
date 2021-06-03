using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Saleos
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var seed = args.Contains("/seed");
            if (seed)
            {
                args = args.Except(new[] {"/seed"}).ToArray();
            }

            var host = CreateHostBuilder(args).Build();

            if (seed)
            {
                Console.WriteLine("Seeding database...");
                SeedData.EnsureSeedData(host.Services).Wait();
                Console.WriteLine("Done seeding database.");
                return ;
            }

            Console.WriteLine("Starting host...");
            host.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}