using System;
using System.Collections.Generic;
using System.Linq;
using SlowPochta.Business.Module.Configuration;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SlowPochta.Business.Module.Modules
{
	public class WebSocketsRoutesManager : IDisposable
	{
		private readonly Dictionary<string, IWebSocketBehaviorContainer> _pathToProcessors;
		private readonly WebSocketServer _webSocketServer;

		public WebSocketsRoutesManager(IEnumerable<IWebSocketBehaviorContainer> webSocketRequestProcessors, WebSocketsRoutesManagerConfig config)
		{
			_webSocketServer = new WebSocketServer(config.WebSocketUrl);
			_pathToProcessors = webSocketRequestProcessors.ToDictionary(processor => processor.Path, processor => processor);
		}

		public void Start()
		{
			_webSocketServer.Start();
		}

		public void Dispose()
		{
			_webSocketServer.Stop();
		}

		public void AddWebSocketService<TBehavior>(string path) where TBehavior : WebSocketBehavior, new()
		{
			_webSocketServer.AddWebSocketService<TBehavior>(path);
		}
	}
}
