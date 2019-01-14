using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SlowPochta.Business.Module.Modules
{
	public class WebSocketsModule
	{
		private readonly Dictionary<string, IWebSocketBehavior> _pathToProcessors;

		public WebSocketsModule(IEnumerable<IWebSocketBehavior> webSocketRequestProcessors)
		{
			_pathToProcessors = webSocketRequestProcessors.ToDictionary(processor => processor.Path, processor => processor);
		}

		public async Task ProcessWebSocketRequest(HttpContext context, Func<Task> next)
		{
			if (_pathToProcessors.TryGetValue(context.Request.Path, out var wsProcessor))
			{
				if (context.WebSockets.IsWebSocketRequest)
				{
					WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
					await wsProcessor.OnMessage(context, webSocket);
				}
				else
				{
					context.Response.StatusCode = 400;
				}
			}
			else
			{
				await next();
			}
		}
	}
}
