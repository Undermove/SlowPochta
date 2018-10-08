using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace SlowPochta.Api.Configuration
{
	public class AuthOptions
	{
		public virtual string Issuer { get; }
		public virtual string Audience { get; }
		public virtual string Key { get; }
		public virtual int LifetimeMinutes { get; }
		public virtual SymmetricSecurityKey SymmetricSecurityKey => GetSymmetricSecurityKey(Key);

		private const string AuthSection = "Auth";

		public AuthOptions(IConfigurationRoot configuration)
		{
			Issuer = configuration[$"{AuthSection}:Issuer"];
			Audience = configuration[$"{AuthSection}:Audience"];
			Key = configuration[$"{AuthSection}:Key"];
			LifetimeMinutes = Convert.ToInt32(configuration[$"{AuthSection}:Lifetime"]);
		}

		private SymmetricSecurityKey GetSymmetricSecurityKey(string key)
		{
			return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
		}
	}
}
