using ServerLogic.Interfaces;
using Commons;
using System.Collections.Concurrent;
using System.Net;
using ServerLogic;

namespace ServerPresentation
{
    public class ElectionServer
    {
        private readonly HttpListener _listener = new();
        private readonly IElectionSystem _electionSystem;
        private readonly JsonSerializer _serializer = new();
        private readonly ConcurrentDictionary<WebSocketConnection, IDisposable> _subscriptions = new();
        private readonly string ServerAdress = "http://localhost:5000/";

        public ElectionServer()
        {
            _electionSystem = LogicAbstractAPI.Create().GetElectionSystem();
            _listener.Prefixes.Add(ServerAdress);
            Logger.Info("ElectionServer initialized.");
        }

        public async Task StartAsync()
        {
            _listener.Start();
            Logger.Info($"ElectionServer started and listening on {ServerAdress}");

            while (true)
            {
                var context = await _listener.GetContextAsync();
                if (!context.Request.IsWebSocketRequest)
                {
                    Logger.Warning("Received non-WebSocket request. Returning 400.");
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                    continue;
                }

                Logger.Info("WebSocket connection request received.");
                _ = HandleWebSocketAsync(context);
            }
        }

        private async Task HandleWebSocketAsync(HttpListenerContext context)
        {
            try
            {
                var wsCtx = await context.AcceptWebSocketAsync(null);
                var connection = new WebSocketConnection(wsCtx.WebSocket);

                Logger.Info("WebSocket client connected.");

                var connMsg = new ConnectionMessage
                {
                    Action = "MakeConnection",
                    ElectionName = _electionSystem.GetElectionTitle()
                };
                await connection.SendMessageAsync(_serializer.Serialize(connMsg));
                Logger.Debug("Sent initialization message to client.");

                var subscription = _electionSystem.Subscribe(new WebSocketObserver(connection, _serializer));
                _subscriptions[connection] = subscription;
                Logger.Debug("Client subscribed to vote updates.");

                connection.OnMessageReceived += async (raw) =>
                {
                    Logger.Debug($"Received raw message: {raw}");
                    var action = _serializer.GetAction(raw);
                    Logger.Info($"Action received: {action}");

                    switch (action)
                    {
                        case "CastVote":
                            var vote = _serializer.Deserialize<VoteRequestMessage>(raw);
                            _electionSystem.CastVote(vote.CandidateId, vote.AuthCode);
                            Logger.Info($"Vote cast for candidate: {vote.CandidateId}");
                            break;

                        case "Unsubscribe":
                            if (_subscriptions.TryRemove(connection, out var sub))
                            {
                                sub.Dispose();
                                Logger.Info("Client unsubscribed.");
                            }
                            break;

                        default:
                            Logger.Warning($"Unknown action received: {action}");
                            break;
                    }
                };

                await connection.ProcessMessagesAsync();

                if (_subscriptions.TryRemove(connection, out var unsub))
                {
                    unsub.Dispose();
                    Logger.Info("WebSocket connection closed and unsubscribed.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception in HandleWebSocketAsync: {ex.Message}");
                context.Response.StatusCode = 500;
                context.Response.Close();
            }
        }
    }
}
