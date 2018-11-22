using System;
using Microsoft.Extensions.Configuration;

namespace SlowPochta.Business.Module.Configuration
{
	public class MessageStatusUpdaterConfig
	{
		public virtual int UpdateIntervalSeconds { get; }

		private const string MessageStatusUpdaterSection = "MessageStatusUpdater";

		public MessageStatusUpdaterConfig(IConfigurationRoot configuration)
		{
			string key = MessageStatusUpdaterSection + ":" + nameof(UpdateIntervalSeconds);
			UpdateIntervalSeconds = Int32.Parse(configuration[key]);
		}
	}
}
