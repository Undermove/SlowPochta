using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SlowPochta.JwtTokenGenerator.Util
{
	class Program
	{
		static void Main(string[] args)
		{
			DateTime now = DateTime.UtcNow;

			Console.WriteLine("Enter security key (TestKeyTestKeyTestKey by default).");
			var securityKey = Console.ReadLine();

			if (securityKey == String.Empty)
			{
				securityKey = "TestKeyTestKeyTestKey";
			}

			Console.WriteLine("Enter expiration time in minutes. (+10 years by default)");

			Int32.TryParse(Console.ReadLine(), out var expirationTime);

			var jwt = new JwtSecurityToken(
				issuer: "server",
				audience: "server",
				notBefore: now,
				expires: expirationTime > 0 ? now.Add(TimeSpan.FromMinutes(expirationTime)) : now.AddYears(10),
				signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256));
			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			Console.WriteLine(encodedJwt);
			Console.ReadLine();
		}

		public static SymmetricSecurityKey GetSymmetricSecurityKey(string key)
		{
			return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
		}
	}
}
