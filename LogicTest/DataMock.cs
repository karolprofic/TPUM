using Data;
using Data.Interfaces;

namespace LogicTest
{
    public class DataMock : DataAbstractAPI
    {
        private readonly ElectionMock election;

        public DataMock()
        {
            election = new ElectionMock();
        }

        public override IElection GetElection()
        {
            return election;
        }

    }

    public class ElectionMock : IElection
    {
        public event EventHandler<VotesChangeEventArgs> VotesChange = delegate { };
        private readonly List<ICandidate> candidates = [
            new Candidate("Jan", "Kowalski", 0),
            new Candidate("Anna", "Nowak", 0),
            new Candidate("Piotr", "Wiśniewski", 0),
            new Candidate("Maria", "Wiśniewska", 0)
        ];
        private readonly HashSet<string> availableCodes = ["123456", "234567", "345678", "456789", "567890"];

        public List<Candidate> GetAllCandidates()
        {
            return candidates.Select(c => (Candidate)c.Clone()).ToList();
        }

        public Candidate GetCandidateById(Guid id)
        {
            var candidate = candidates.FirstOrDefault(c => c.Id == id);
            return candidate == null ? throw new KeyNotFoundException("Candidate not found.") : (Candidate)candidate.Clone();
        }

        public string GetElectionTitle()
        {
            return "Wybory Testowe";
        }

        public void Vote(Guid candidateId, string code)
        {
            if (!availableCodes.Contains(code))
            {
                return;
            }
            foreach (var candidate in candidates)
            {
                if (candidate.Id == candidateId)
                {
                    candidate.AddVotes(1);
                    availableCodes.Remove(code);
                    OnVotesChanged(candidate.Id, candidate.Votes);
                    return;
                }
            }
        }

        public void SimulateVote()
        {
            foreach (var candidate in candidates)
            {
                candidate.AddVotes(1);
                OnVotesChanged(candidate.Id, candidate.Votes);
            }
        }

        private void OnVotesChanged(Guid id, int votes)
        {
            VotesChange?.Invoke(this, new VotesChangeEventArgs(id, votes));
        }
    }

}
