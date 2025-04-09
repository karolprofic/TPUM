using ClientData;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
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
        private const string ServerUrl = "ws://localhost:5000/";

        private readonly ElectionClientConnection connection;

        public Election()
        {
            connection = new ElectionClientConnection(ServerUrl);
            connection.OnMessageReceived += HandleMessageAsync;
            _ = connection.ConnectAsync();
        }

        private async Task HandleMessageAsync(string json)
        {
            try
            {
                JsonNode? payload = JsonNode.Parse(json);
                string action = payload?["Action"]?.ToString();
                if (string.IsNullOrEmpty(action))
                {
                    return;
                }

                if (action == "MakeConnection")
                {
                    electionTitle = payload["ElectionName"]?.ToString() ?? electionTitle;
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CLIENT] Error during message processing: {ex.Message}");
            }

            await Task.CompletedTask;
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
            _ = connection.SendVoteAsync(candidateId, code);
        }

        private void OnVotesChanged(Guid id, int votes)
        {
            VotesChange?.Invoke(this, new VotesChangeEventArgs(id, votes));
        }

    }

}
