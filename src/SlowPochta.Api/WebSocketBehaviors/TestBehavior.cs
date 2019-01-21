using System;
using SlowPochta.Business.Module.Modules;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SlowPochta.Api.WebSocketBehaviors
{
	public class TestBehavior : WebSocketBehavior, IWebSocketBehaviorContainer
	{
		public string Path { get; } = "/test";

		public WebSocketBehavior Behavior { get; }

		public TestBehavior()
		{
			Behavior = this;
		}

		protected override void OnClose(CloseEventArgs e)
		{
			base.OnClose(e);
		}

		protected override void OnError(ErrorEventArgs e)
		{
			base.OnError(e);
		}

		protected override void OnMessage(MessageEventArgs e)
		{
			var msg = e.Data == "BALUS"
				? "I've been balused already..."
				: "I'm not available now.";
			Console.WriteLine(msg);
			Send(msg);
		}

		protected override void OnOpen()
		{
			base.OnOpen();
		}
	}
}
