using Newtonsoft.Json;

namespace SlowPochta.Business.Module.WebSocket
{
	public class WebSocketAuthHeader
	{
		[JsonProperty("token")]
		public string Token { get; set; }
	}
}
