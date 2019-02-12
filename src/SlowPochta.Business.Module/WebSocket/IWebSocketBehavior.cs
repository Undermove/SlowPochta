using WebSocketSharp.Server;

namespace SlowPochta.Business.Module.WebSocket
{
	public interface IWebSocketBehaviorContainer
	{
		string Path { get; }

		WebSocketBehavior Behavior { get; }
	}
}