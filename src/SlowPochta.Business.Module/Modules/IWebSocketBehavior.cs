using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SlowPochta.Business.Module.Modules
{
	public interface IWebSocketBehavior
	{
		string Path { get; }

		Task OnMessage(HttpContext context, WebSocket webSocket);
	}
}