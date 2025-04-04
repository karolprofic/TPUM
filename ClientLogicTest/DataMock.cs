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
            new CandidateMock("Jan", "Kowalski", 0),
            new CandidateMock("Anna", "Nowak", 0),
            new CandidateMock("Piotr", "Wiśniewski", 0),
            new CandidateMock("Maria", "Wiśniewska", 0)
        ];
        private readonly HashSet<string> availableCodes = ["123456", "234567", "345678", "456789", "567890"];

        public List<ICandidate> GetAllCandidates()
        {
            return candidates.Select(c => (ICandidate)c.Clone()).ToList();
        }

        public ICandidate GetCandidateById(Guid id)
        {
            var candidate = candidates.FirstOrDefault(c => c.Id == id);
            return candidate == null ? throw new KeyNotFoundException("Candidate not found.") : (ICandidate)candidate.Clone();
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

    public class CandidateMock : ICandidate, ICloneable
    {
        public Guid Id { get; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public int Votes { get; private set; }

        public CandidateMock(string name, string surname, int votes)
        {
            Id = Guid.NewGuid();
            Name = name;
            Surname = surname;
            Votes = votes;
        }

        private CandidateMock(Guid id, string name, string surname, int votes)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Votes = votes;
        }

        public object Clone()
        {
            return new CandidateMock(Id, Name, Surname, Votes);
        }

        public void AddVotes(int count)
        {
            if (count > 0)
            {
                Votes += count;
            }
        }

    }

}
