using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ToDoApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://*:5000")  // listen on port 5000 on all network interfaces; needed for containers
                .UseContentRoot(Directory.GetCurrentDirectory())
                .CaptureStartupErrors(true)
                .UseSetting("detailedErrors", "true")
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}


