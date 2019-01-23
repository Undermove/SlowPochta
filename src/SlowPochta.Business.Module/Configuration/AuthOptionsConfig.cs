using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace SlowPochta.Business.Module.Configuration
{
	public class AuthOptionsConfig
	{
		public virtual string Issuer { get; }
		public virtual string Audience { get; }
		public virtual string Key { get; }
		public virtual int LifetimeMinutes { get; }
		public virtual SymmetricSecurityKey SymmetricSecurityKey => GetSymmetricSecurityKey(Key);

		private const string AuthSection = "Auth";

		public AuthOptionsConfig(IConfigurationRoot configuration)
		{
			Issuer = configuration[$"{AuthSection}:Issuer"];
			Audience = configuration[$"{AuthSection}:Audience"];
			Key = configuration[$"{AuthSection}:Key"];
			LifetimeMinutes = Convert.ToInt32(configuration[$"{AuthSection}:LifetimeMinutes"]);
		}

		private SymmetricSecurityKey GetSymmetricSecurityKey(string key)
		{
			return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
		}
	}
}
