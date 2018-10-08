using System;
using Microsoft.Extensions.Configuration;

namespace SlowPochta.Data
{
	public class ConnectionStringProvider
	{
		public virtual string ConnectionString { get; }

		public ConnectionStringProvider(IConfigurationRoot configuration)
		{
			ConnectionString = configuration["ConnectionString"];
		}
	}
}
