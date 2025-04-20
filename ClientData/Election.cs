using ClientData;
using Commons;
using Data.Interfaces;

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
        private readonly Commons.JsonSerializer _serializer = new Commons.JsonSerializer();

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
                var baseMsg = _serializer.Deserialize<BaseMessage>(json);
                if (baseMsg == null || string.IsNullOrEmpty(baseMsg.Action))
                    return;

                switch (baseMsg.Action)
                {
                    case "MakeConnection":
                        {
                            var conn = _serializer.Deserialize<ConnectionMessage>(json)!;
                            electionTitle = conn.ElectionName;
                            break;
                        }
                    case "SendCandidates":
                        {
                            var candidatesMsg = _serializer.Deserialize<CandidatesMessage>(json)!;
                            lock (candidatesLock)
                            {
                                candidates.Clear();
                                foreach (var dto in candidatesMsg.Candidates)
                                {
                                    var c = new Candidate(
                                        dto.Id,
                                        dto.Name,
                                        dto.Surname,
                                        dto.Votes
                                    );
                                    candidates[dto.Id] = c;
                                    OnVotesChanged(dto.Id, dto.Votes);
                                }
                            }
                            break;
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
