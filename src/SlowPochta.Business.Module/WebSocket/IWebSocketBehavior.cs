using WebSocketSharp.Server;

namespace SlowPochta.Business.Module.Modules
{
	public interface IWebSocketBehaviorContainer
	{
		string Path { get; }

		WebSocketBehavior Behavior { get; }
	}
}