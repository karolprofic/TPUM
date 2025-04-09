using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ServerLogic;
using ServerLogic.Interfaces;

namespace ServerPresentation
{
    public class ElectionServer
    {
        public event EventHandler<VotesChangeEventArgs> VotesChanged;

        private readonly HttpListener _listener = new HttpListener();
        private LogicAbstractAPI logicAbstractAPI;
        private IElectionSystem electionSystem;
        private readonly ConcurrentBag<WebSocketConnection> _clients = new ConcurrentBag<WebSocketConnection>();

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
            VotesChanged?.Invoke(this, new VotesChangeEventArgs());
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
            try
            {
                var wsContext = await context.AcceptWebSocketAsync(null);
                var wsConnection = new WebSocketConnection(wsContext.WebSocket);
                _clients.Add(wsConnection);
                Console.WriteLine("[INFO] New WebSocket client connected.");

                wsConnection.OnMessageReceived += async (message) =>
                {
                    await HandleActionAsync(wsConnection, message);
                };

                await wsConnection.SendMessageAsync(new
                {
                    Action = "MakeConnection",
                    ElectionName = electionSystem.GetElectionTitle()
                });

                await wsConnection.SendMessageAsync(new
                {
                    Action = "SendCandidates",
                    Candidates = electionSystem.GetCandidates()
                });

                Console.WriteLine($"[INFO] Sent election name: {electionSystem.GetElectionTitle()}");

                await wsConnection.ProcessMessagesAsync();
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

            foreach (var client in _clients)
            {
                await client.SendMessageAsync(new
                {
                    Action = "SendCandidates",
                    Candidates = candidates
                });
            }
        }

        private async Task HandleActionAsync(WebSocketConnection connection, string json)
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
                    await connection.SendMessageAsync(new
                    {
                        Action = "SendCandidates",
                        Candidates = candidates
                    });
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
                        await connection.SendMessageAsync(new
                        {
                            Action = "SendCandidates",
                            Candidates = candidates
                        });
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
    }
}
