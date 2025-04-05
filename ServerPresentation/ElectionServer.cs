using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ServerLogic;
using ServerLogic.Interfaces;

namespace ServerPresentation
{
    public class ElectionServer
    {
        private readonly HttpListener _listener = new HttpListener();

        private LogicAbstractAPI logicAbstractAPI;
        private IElectionSystem electionSystem;

        public ElectionServer(string url)
        {
            logicAbstractAPI = LogicAbstractAPI.Create();
            electionSystem = logicAbstractAPI.GetElectionSystem();
            _listener.Prefixes.Add(url);
        }

        public async Task StartAsync()
        {
            _listener.Start();
            Console.WriteLine("Server start");

            while (true)
            {
                var context = await _listener.GetContextAsync();
                if (!context.Request.IsWebSocketRequest)
                {
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
                Console.WriteLine("New WebSocket connection");

                await SendMessageAsync(webSocket, new
                {
                    Action = "MakeConnection",
                    ElectionName = electionSystem.GetElectionTitle()
                });
                await ProcessMessagesAsync(webSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                context.Response.StatusCode = 500;
                context.Response.Close();
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
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    Console.WriteLine("Connection close");
                    break;
                }

                string json = Encoding.UTF8.GetString(buffer, 0, result.Count);
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
                    return;
                }

                string action = actionElem.GetString();

                if (action == "RequestCandidates")
                {
                    Console.WriteLine("Request type: RequestCandidates");
                    List<CandidateDTO> candidates = electionSystem.GetCandidates();
                    await SendMessageAsync(webSocket, new { Action = "SendCandidates", Candidates = candidates });
                }

                if (action == "CastVode")
                {
                    Console.WriteLine("Request type: CastVode");
                    if (doc.RootElement.TryGetProperty("CandidateId", out JsonElement candidateIdElem) &&
                        doc.RootElement.TryGetProperty("AuthCode", out JsonElement authCodeElem))
                    {
                        Guid candidateId = candidateIdElem.GetGuid();
                        string authCode = authCodeElem.GetString();
                        electionSystem.CastVote(candidateId, authCode);
                        List<CandidateDTO> candidates = electionSystem.GetCandidates();
                        await SendMessageAsync(webSocket, new { Action = "SendCandidates", Candidates = candidates });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during request processing: " + ex.Message);
            }
        }

        private async Task SendMessageAsync(WebSocket webSocket, object messageObj)
        {
            string json = JsonSerializer.Serialize(messageObj);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
