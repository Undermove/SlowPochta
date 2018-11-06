using System;
using Microsoft.Extensions.Configuration;

namespace SlowPochta.Business.Module.Configuration
{
	public class MessageStatusUpdaterConfig
	{
		public virtual int UpdateIntervalMinutes { get; }

		private const string MessageStatusUpdaterSection = "MessageStatusUpdater";

		public MessageStatusUpdaterConfig(IConfigurationRoot configuration)
		{
			UpdateIntervalMinutes = Int32.Parse(configuration[MessageStatusUpdaterSection] + ":" + nameof(UpdateIntervalMinutes));
		}
	}
}
