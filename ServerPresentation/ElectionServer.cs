using System.Collections.Concurrent;
using System.Net;
using Commons;
using ServerLogic;
using ServerLogic.Interfaces;

namespace ServerPresentation
{
    public class ElectionServer
    {
        public event EventHandler<VotesChangeEventArgs> VotesChanged;

        private readonly HttpListener _listener = new HttpListener();
        private readonly LogicAbstractAPI _logicAbstractAPI;
        private readonly IElectionSystem _electionSystem;
        private readonly JsonSerializer _serializer = new JsonSerializer();
        private readonly ConcurrentBag<WebSocketConnection> _clients = new ConcurrentBag<WebSocketConnection>();

        public ElectionServer()
        {
            _logicAbstractAPI = LogicAbstractAPI.Create();
            _electionSystem = _logicAbstractAPI.GetElectionSystem();
            _electionSystem.VotesChange += OnVotesChanged;
            _listener.Prefixes.Add("http://localhost:5000/");
        }

        private void OnVotesChanged(object? sender, ServerLogic.VotesChangeEventArgs e)
        {
            Console.WriteLine("[LOGIC] Invoke VotesChangeEventArgs");
            VotesChanged?.Invoke(this, new VotesChangeEventArgs());
            _ = SendCandidatesToAllClientsAsync();
        }

        public async Task StartAsync()
        {
            _listener.Start();
            Console.WriteLine("[INFO] Election server started and listening for connections...");

            while (true)
            {
                var context = await _listener.GetContextAsync();
                if (!context.Request.IsWebSocketRequest)
                {
                    Console.WriteLine("[WARN] Received non-WebSocket request. Ignoring...");
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                    continue;
                }
                _ = HandleWebSocketAsync(context);
            }
        }

        private async Task HandleWebSocketAsync(HttpListenerContext context)
        {
            try
            {
                var wsContext = await context.AcceptWebSocketAsync(null);
                var wsConnection = new WebSocketConnection(wsContext.WebSocket);
                _clients.Add(wsConnection);
                Console.WriteLine("[INFO] New WebSocket client connected.");

                wsConnection.OnMessageReceived += async (raw) =>
                {
                    await HandleIncomingAsync(wsConnection, raw);
                };

                var connMsg = new ConnectionMessage
                {
                    Action = "MakeConnection",
                    ElectionName = _electionSystem.GetElectionTitle()
                };
                await wsConnection.SendMessageAsync(_serializer.Serialize(connMsg));

                await SendCandidatesToClientAsync(wsConnection);
                await wsConnection.ProcessMessagesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] WebSocket handshake failed: {ex.Message}");
                context.Response.StatusCode = 500;
                context.Response.Close();
            }
        }

        private async Task SendCandidatesToClientAsync(WebSocketConnection client)
        {
            var dto = new CandidatesMessage
            {
                Action = "SendCandidates",
                Candidates = _electionSystem.GetCandidates()
            };
            await client.SendMessageAsync(_serializer.Serialize(dto));
        }

        private async Task SendCandidatesToAllClientsAsync()
        {
            var dto = new CandidatesMessage
            {
                Action = "SendCandidates",
                Candidates = _electionSystem.GetCandidates()

            };

            var json = _serializer.Serialize(dto);
            Console.WriteLine($"[INFO] Broadcasting candidates to {_clients.Count} clients.");

            foreach (var client in _clients)
                await client.SendMessageAsync(json);
        }

        private async Task HandleIncomingAsync(WebSocketConnection connection, string rawJson)
        {
            try
            {
                var header = _serializer.GetAction(rawJson);
                if (string.IsNullOrEmpty(header))
                {
                    Console.WriteLine("[WARN] No Header in incoming message.");
                    return;
                }

                Console.WriteLine($"[INFO] Received header: {header}");
                switch (header)
                {
                    case "RequestCandidates":
                        await SendCandidatesToClientAsync(connection);
                        break;

                    case "CastVote":
                        var voteReq = _serializer.Deserialize<VoteRequestMessage>(rawJson);
                        if (voteReq != null)
                        {
                            Console.WriteLine($"[INFO] Casting vote for {voteReq.CandidateId}");
                            _electionSystem.CastVote(voteReq.CandidateId, voteReq.AuthCode);
                            await SendCandidatesToClientAsync(connection);
                        }
                        break;

                    default:
                        Console.WriteLine($"[WARN] Unknown Action Type: {header}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Handling incoming JSON failed: {ex.Message}");
            }
        }

    }
}
