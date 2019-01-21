using System;
using Microsoft.Extensions.Configuration;

namespace SlowPochta.Business.Module.Configuration
{
	public class WebSocketsRoutesManagerConfig
	{
		public virtual string WebSocketUrl { get; }

		private const string WebSocketsRoutesManagerConfigSection = "WebSocketsRoutesManager";

		public WebSocketsRoutesManagerConfig(IConfigurationRoot configuration)
		{
			string key = WebSocketsRoutesManagerConfigSection + ":" + nameof(WebSocketUrl);
			WebSocketUrl = configuration[key];
		}
	}
}
