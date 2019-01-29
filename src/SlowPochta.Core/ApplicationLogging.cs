using Microsoft.Extensions.Logging;

namespace SlowPochta.Core
{
	public static class ApplicationLogging
	{
		public static ILoggerFactory LoggerFactory { get; set; } //= new LoggerFactory();
		public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
		public static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);
	}
}
