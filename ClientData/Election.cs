using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Data
{
    internal class Election : IElection
    {
        public event EventHandler<VotesChangeEventArgs> VotesChange;
        private readonly Dictionary<Guid, Candidate> candidates = new Dictionary<Guid, Candidate>();
        private readonly object candidatesLock = new object();
        private readonly HashSet<string> availableCodes = new HashSet<string>();
        private readonly object votingLock = new object();
        private string electionTitle;

        public Election()
        {
            electionTitle = "Wybory Prezydenckie 2025";

            var candidate = new Candidate("Jan", "Kowalski", 0);
            candidates.Add(candidate.Id, candidate);
            candidate = new Candidate("Anna", "Nowak", 0);
            candidates.Add(candidate.Id, candidate);
            candidate = new Candidate("Piotr", "Wiśniewski", 0);
            candidates.Add(candidate.Id, candidate);
            candidate = new Candidate("Maria", "Wiśniewska", 0);
            candidates.Add(candidate.Id, candidate);
            candidate = new Candidate("Tomasz", "Zieliński", 0);
            candidates.Add(candidate.Id, candidate);
            candidate = new Candidate("Agnieszka", "Kamińska", 0);
            candidates.Add(candidate.Id, candidate);
            candidate = new Candidate("Robert", "Lewandowski", 0);
            candidates.Add(candidate.Id, candidate);
            candidate = new Candidate("Ewa", "Kowalczyk", 0);
            candidates.Add(candidate.Id, candidate);

            availableCodes.Add("123456");
            availableCodes.Add("234567");
            availableCodes.Add("345678");
            availableCodes.Add("456789");
            availableCodes.Add("567890");
            availableCodes.Add("678901");
            availableCodes.Add("789012");
            availableCodes.Add("890123");
            availableCodes.Add("901234");
            availableCodes.Add("012345");

        }

        ~Election()
        {
            lock (votingLock) { }
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
            lock (candidatesLock)
            {
                if (!availableCodes.Contains(code))
                {
                    return;
                }

                foreach (var candidate in candidates.Values)
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
        }

        private void OnVotesChanged(Guid id, int votes)
        {
            EventHandler<VotesChangeEventArgs> handler = VotesChange;
            handler?.Invoke(this, new VotesChangeEventArgs(id, votes));
        }

    }
}
