using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SlowPochta.Business.Module.Modules;
using SlowPochta.Business.Module.Services;
using SlowPochta.Business.Module.WebSocket;
using SlowPochta.Core;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SlowPochta.Api.WebSocketBehaviors
{
	public class MessageNotificationBehavior :  WebSocketBehavior, IWebSocketBehaviorContainer, IDisposable
	{
		private static readonly ILogger Logger = ApplicationLogging.CreateLogger<MessageNotificationBehavior>();

		private readonly AuthModule _authModule;
		private readonly MessageModule _messageModule;
		private readonly MessageStatusUpdater _messageStatusUpdater;
		private readonly Dictionary<string, string> _loginsToSessionIds = new Dictionary<string, string>();
		private readonly Dictionary<string, string> _sessionIdToLogin = new Dictionary<string, string>();

		public MessageNotificationBehavior(
			AuthModule authModule,
			MessageModule messageModule,
			MessageStatusUpdater messageStatusUpdater)
		{
			_authModule = authModule;
			_messageModule = messageModule;
			_messageModule.MessageMarkedRead += OnMessageMarkedRead;

			_messageStatusUpdater = messageStatusUpdater;
			_messageStatusUpdater.MessageDelivered += OnMessageDelivered;

			Behavior = this;
		}

		public void Dispose()
		{
			_messageStatusUpdater.MessageDelivered -= OnMessageDelivered;
			_messageModule.MessageMarkedRead -= OnMessageMarkedRead;

			_authModule?.Dispose();
			_messageModule?.Dispose();
		}

		public string Path { get; } = "/auth";

		public WebSocketBehavior Behavior { get; }

		protected override void OnMessage(MessageEventArgs e)
		{
			var login = _authModule.GetLoginFromToken(e.Data);
			if (login != null)
			{
				_sessionIdToLogin[ID] = login;
				_loginsToSessionIds[login] = ID;

				return;
			}

			Logger.LogDebug($"Error in auth. Close websocket session {ID}");
			Sessions.CloseSession(ID);
		}

		protected override void OnClose(CloseEventArgs e)
		{
			if (_sessionIdToLogin.Remove(ID, out var login))
			{
				_loginsToSessionIds.Remove(login);
			}

			base.OnClose(e);
		}

		private async Task OnMessageDelivered(object sender, MessageDeliveredEventArgs e)
		{
			try
			{
				List<string> login = await _messageModule.GetRecieversUsersLogins(e.MessageId);

				if (_loginsToSessionIds.TryGetValue(login.FirstOrDefault(), out var sessionId))
				{
					Sessions[sessionId].Context.WebSocket.Send(JsonConvert.SerializeObject(new { notification = "new message" }));
				}
			}
			catch (Exception exception)
			{
				Logger.LogCritical(exception, "Something went wrong during notification handling");
			}
		}

		private async Task OnMessageMarkedRead(object sender, MessageDeliveredEventArgs e)
		{
			try
			{
				List<string> login = await _messageModule.GetRecieversUsersLogins(e.MessageId);

				if (_loginsToSessionIds.TryGetValue(login.FirstOrDefault(), out var sessionId))
				{
					Sessions[sessionId].Context.WebSocket.Send(JsonConvert.SerializeObject(new { notification = "message marked read" }));
				}
			}
			catch (Exception exception)
			{
				Logger.LogCritical(exception, "Something went wrong during notification handling");
			}
		}
	}
}
