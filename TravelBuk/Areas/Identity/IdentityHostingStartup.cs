using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(TravelBuk.Areas.Identity.IdentityHostingStartup))]
namespace TravelBuk.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}