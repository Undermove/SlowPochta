using System;
using SlowPochta.Business.Module.Modules;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SlowPochta.Api.WebSocketBehaviors
{
	public class AuthSocketBehavior : WebSocketBehavior, IWebSocketBehaviorContainer
	{
		private readonly AuthModule _authModule;
		public string Path { get; } = "/auth";

		public WebSocketBehavior Behavior { get; }

		public AuthSocketBehavior(AuthModule authModule)
		{
			_authModule = authModule;
			Behavior = this;
		}

		protected override void OnMessage(MessageEventArgs e)
		{
			var claim = _authModule.CheckJwtToken(e.Data);
			if (claim != null)
			{
				_authModule.CheckJwtToken(e.Data);
				var msg = e.Data == "BALUS"
					? "I've been balused already..."
					: "I'm not available now.";
				Console.WriteLine(msg);
				Send(msg);
				Console.WriteLine(Sessions.ActiveIDs);
				return;
			}

			Console.WriteLine("Error in auth");
			Sessions.CloseSession(ID);
		}
	}
}
