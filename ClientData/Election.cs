using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Data
{
    internal class Election : IElection
    {
        public event EventHandler<VotesChangeEventArgs> VotesChange;

        private readonly Dictionary<Guid, Candidate> candidates = new Dictionary<Guid, Candidate>();
        private readonly object candidatesLock = new object();
        private string electionTitle = "Ładowanie...";

        // Adres serwera WebSocket – dostosuj w razie potrzeby.
        private const string ServerUrl = "ws://localhost:5000/";

        private readonly ElectionClientConnection connection;

        public Election()
        {
            connection = new ElectionClientConnection(ServerUrl);
            connection.OnMessageReceived += HandleServerMessage;
            // Inicjujemy połączenie asynchronicznie – fire-and-forget
            _ = connection.ConnectAsync();
        }

        private void HandleServerMessage(string action, JsonNode payload)
        {
            if (action == "MakeConnection")
            {
                // Aktualizacja tytułu wyborów
                electionTitle = payload["ElectionName"]?.ToString() ?? electionTitle;
                Console.WriteLine($"[CLIENT] Otrzymano tytuł wyborów: {electionTitle}");
            }
            else if (action == "SendCandidates")
            {
                var candidatesArray = payload["Candidates"]?.AsArray();
                if (candidatesArray == null) return;

                lock (candidatesLock)
                {
                    candidates.Clear();
                    foreach (var item in candidatesArray)
                    {
                        Guid id = Guid.Parse(item["Id"].ToString());
                        string firstName = item["Name"].ToString();
                        string lastName = item["Surname"].ToString();
                        int votes = int.Parse(item["Votes"].ToString());
                        var candidate = new Candidate(id, firstName, lastName, votes);
                        candidates[id] = candidate;
                        OnVotesChanged(id, votes);
                    }
                }
                Console.WriteLine($"[CLIENT] Odebrano i zaktualizowano listę kandydatów ({candidates.Count} pozycji).");
            }
            else
            {
                Console.WriteLine($"[CLIENT] Nieznana akcja: {action}");
            }
        }

        public List<ICandidate> GetAllCandidates()
        {
            lock (candidatesLock)
            {
                return candidates.Values.Select(c => (ICandidate)c.Clone()).ToList();
            }
        }

        public string GetElectionTitle()
        {
            return electionTitle;
        }

        public void Vote(Guid candidateId, string code)
        {
            Console.WriteLine($"[CLIENT] Wysyłanie głosu dla kandydata {candidateId} z kodem {code}.");
            _ = connection.SendVoteAsync(candidateId, code);
        }

        private void OnVotesChanged(Guid id, int votes)
        {
            VotesChange?.Invoke(this, new VotesChangeEventArgs(id, votes));
        }

        private class ElectionClientConnection
        {
            private readonly string url;
            private ClientWebSocket webSocket;

            public event Action<string, JsonNode> OnMessageReceived;

            public ElectionClientConnection(string url)
            {
                this.url = url;
            }

            public async Task ConnectAsync()
            {
                try
                {
                    webSocket = new ClientWebSocket();
                    await webSocket.ConnectAsync(new Uri(url), CancellationToken.None);
                    Console.WriteLine("[CLIENT] Połączono z serwerem WebSocket.");
                    _ = ReceiveLoopAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CLIENT] Błąd podczas łączenia: {ex.Message}");
                }
            }

            private async Task ReceiveLoopAsync()
            {
                var buffer = new byte[4096];
                while (webSocket.State == WebSocketState.Open)
                {
                    try
                    {
                        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            Console.WriteLine("[CLIENT] Serwer zamknął połączenie.");
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                            break;
                        }

                        string json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var obj = JsonNode.Parse(json);
                        string action = obj?["Action"]?.ToString();
                        if (!string.IsNullOrEmpty(action))
                        {
                            OnMessageReceived?.Invoke(action, obj);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[CLIENT] Błąd podczas odbioru wiadomości: {ex.Message}");
                    }
                }
            }

            public async Task SendVoteAsync(Guid candidateId, string code)
            {
                var messageObj = new
                {
                    Action = "CastVote",
                    CandidateId = candidateId.ToString(),
                    AuthCode = code
                };

                await SendAsync(messageObj);
            }

            private async Task SendAsync(object message)
            {
                try
                {
                    string json = JsonSerializer.Serialize(message);
                    byte[] bytes = Encoding.UTF8.GetBytes(json);
                    await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    Console.WriteLine($"[CLIENT] Wysłano wiadomość: {json}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CLIENT] Błąd podczas wysyłania wiadomości: {ex.Message}");
                }
            }
        }
    }
}