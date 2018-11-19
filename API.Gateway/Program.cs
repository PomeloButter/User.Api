using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Gateway.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((webHost, bulider) =>
                    {
                        bulider.SetBasePath(webHost.HostingEnvironment.ContentRootPath).AddJsonFile("Ocelot.json");
                    })
                .UseStartup<Startup>()
                .Build();
    }
}
