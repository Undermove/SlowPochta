using Newtonsoft.Json;

namespace SlowPochta.Business.Module.Responses
{
	public class AuthResponse
	{
		[JsonProperty("access_token")]
		public string AccessToken { get; }

		[JsonProperty("username")]
		public string Username { get; }

		public AuthResponse(string accessToken, string username)
		{
			AccessToken = accessToken;
			Username = username;
		}
	}
}
