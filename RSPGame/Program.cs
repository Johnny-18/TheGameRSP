using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace RSPGame
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().
                WriteTo.Console().
                WriteTo.File("app.log")
                .CreateLogger();
            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}