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
        public event EventHandler<VotesChangeEventArgs> VotesChange;
        private readonly List<ICandidate> candidates;
        private readonly HashSet<string> availableCodes;

        public ElectionMock()
        {
            candidates = new List<ICandidate>
            {
                new Candidate("Jan", "Kowalski", 0),
                new Candidate("Anna", "Nowak", 0),
                new Candidate("Piotr", "Wiśniewski", 0),
                new Candidate("Maria", "Wiśniewska", 0)
            };

            availableCodes = new HashSet<string>
            {
                "123456", "234567", "345678", "456789", "567890"
            };
        }

        public List<Candidate> GetAllCandidates()
        {
            return candidates.Select(c => (Candidate)c.Clone()).ToList();
        }

        public Candidate GetCandidateById(Guid id)
        {
            var candidate = candidates.FirstOrDefault(c => c.Id == id);
            if (candidate == null)
            {
                throw new KeyNotFoundException("Candidate not found.");
            }
            return (Candidate)candidate.Clone();
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

            var candidate = candidates.FirstOrDefault(c => c.Id == candidateId);
            if (candidate != null)
            {
                candidate.AddVotes(1);
                availableCodes.Remove(code);
                OnVotesChanged(candidate.Id, candidate.Votes);
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

/*    public class CandidateMock : ICandidate, ICloneable
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

*//*        public object Clone()
        {
            Candidate clone = (Candidate)MemberwiseClone();
            clone.Name = string.Copy(Name);
            clone.Surname = string.Copy(Surname);
            return clone;
        }*//*

        public void AddVotes(int count)
        {
            if (count > 0)
            {
                Votes += count;
            }
        }

    }*/

}
