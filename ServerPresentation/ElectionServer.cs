using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ServerLogic;
using ServerLogic.Interfaces;
using System.Collections.Concurrent;

namespace ServerPresentation
{
    public class ElectionServer
    {
        public event EventHandler<VotesChangeEventArgs> VotesChanged;

        private readonly HttpListener _listener = new HttpListener();
        private LogicAbstractAPI logicAbstractAPI;
        private IElectionSystem electionSystem;

        private readonly ConcurrentBag<WebSocket> _clients = new ConcurrentBag<WebSocket>();

        public ElectionServer()
        {
            logicAbstractAPI = LogicAbstractAPI.Create();
            electionSystem = logicAbstractAPI.GetElectionSystem();
            electionSystem.VotesChange += OnVotesChanged;
            _listener.Prefixes.Add("http://localhost:5000/");
        }

        private void OnVotesChanged(object? sender, ServerLogic.VotesChangeEventArgs e)
        {
            Console.WriteLine("[LOGIC] Invoke VotesChangeEventArgs");
            EventHandler<VotesChangeEventArgs> handler = VotesChanged;
            handler?.Invoke(this, new VotesChangeEventArgs());
            _ = SendCandidatesToAllClients();
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
            WebSocket webSocket = null;
            try
            {
                webSocket = (await context.AcceptWebSocketAsync(null)).WebSocket;
                Console.WriteLine("[INFO] New WebSocket client connected.");

                _clients.Add(webSocket);

                await SendMessageAsync(webSocket, new
                {
                    Action = "MakeConnection",
                    ElectionName = electionSystem.GetElectionTitle()
                });

                await SendMessageAsync(webSocket, new 
                { 
                    Action = "SendCandidates", 
                    Candidates = electionSystem.GetCandidates()
                });

                Console.WriteLine($"[INFO] Sent election name: {electionSystem.GetElectionTitle()}");

                await ProcessMessagesAsync(webSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error during WebSocket handshake: {ex.Message}");
                context.Response.StatusCode = 500;
                context.Response.Close();
            }
        }

        private async Task SendCandidatesToAllClients()
        {
            List<CandidateDTO> candidates = electionSystem.GetCandidates();
            Console.WriteLine($"[INFO] Sending {candidates.Count} candidates to all clients.");

            foreach (var webSocket in _clients)
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    await SendMessageAsync(webSocket, new { Action = "SendCandidates", Candidates = candidates });
                }
            }
        }

        private async Task ProcessMessagesAsync(WebSocket webSocket)
        {
            var buffer = new byte[4096];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("[INFO] Client closed the connection.");
                    _clients.TryTake(out _);
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }

                string json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"[DEBUG] Received message: {json}");

                await HandleActionAsync(webSocket, json);
            }
        }

        private async Task HandleActionAsync(WebSocket webSocket, string json)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("Action", out JsonElement actionElem))
                {
                    Console.WriteLine("[WARN] Received message without 'Action' field.");
                    return;
                }

                string action = actionElem.GetString();
                Console.WriteLine($"[INFO] Processing action: {action}");

                if (action == "RequestCandidates")
                {
                    List<CandidateDTO> candidates = electionSystem.GetCandidates();
                    Console.WriteLine($"[INFO] Sending {candidates.Count} candidates to client.");
                    await SendMessageAsync(webSocket, new { Action = "SendCandidates", Candidates = candidates });
                }
                else if (action == "CastVote")
                {
                    if (doc.RootElement.TryGetProperty("CandidateId", out JsonElement candidateIdElem) &&
                        doc.RootElement.TryGetProperty("AuthCode", out JsonElement authCodeElem))
                    {
                        Guid candidateId = candidateIdElem.GetGuid();
                        string authCode = authCodeElem.GetString();

                        Console.WriteLine($"[INFO] Attempting to cast vote for candidate: {candidateId}");
                        electionSystem.CastVote(candidateId, authCode);
                        Console.WriteLine("[INFO] Vote successfully cast.");

                        List<CandidateDTO> candidates = electionSystem.GetCandidates();
                        await SendMessageAsync(webSocket, new { Action = "SendCandidates", Candidates = candidates });
                    }
                    else
                    {
                        Console.WriteLine("[WARN] Missing CandidateId or AuthCode in 'CastVote' request.");
                    }
                }
                else
                {
                    Console.WriteLine($"[WARN] Unknown action received: {action}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Exception while processing action: " + ex.Message);
            }
        }

        private async Task SendMessageAsync(WebSocket webSocket, object messageObj)
        {
            try
            {
                string json = JsonSerializer.Serialize(messageObj);
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                Console.WriteLine($"[DEBUG] Sent message: {json}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Failed to send message to client: " + ex.Message);
            }
        }
    }
}
