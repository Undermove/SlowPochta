using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace SlowPochta.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();

		public static IWebHost BuildWebHost(string[] args)
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("hosting.json", true)
				.Build();

			return WebHost.CreateDefaultBuilder(args)
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseConfiguration(config)
				.UseKestrel()
				.UseStartup<Startup>()
				.Build();
		}
	}
}
