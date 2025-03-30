using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    internal class Election : IElection
    {
        private readonly Dictionary<Guid, Candidate> candidates = new Dictionary<Guid, Candidate>();
        private readonly object candidatesLock = new object();
        private readonly HashSet<string> availableCodes = new HashSet<string>();
        private bool isVotingActive = true;
        private readonly object votingLock = new object();

        public Election()
        {
            AddCandidate(new Candidate("Jan", "Kowalski", 0));
            AddCandidate(new Candidate("Anna", "Nowak", 0));
            AddCandidate(new Candidate("Piotr", "Wiśniewski", 0));
            AddCandidate(new Candidate("Maria", "Wiśniewska", 0));
            AddCandidate(new Candidate("Tomasz", "Zieliński", 0));
            AddCandidate(new Candidate("Agnieszka", "Kamińska", 0));
            AddCandidate(new Candidate("Robert", "Lewandowski", 0));
            AddCandidate(new Candidate("Ewa", "Kowalczyk", 0));

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

            SimulateVoting();
        }

        ~Election()
        {
            isVotingActive = false;
            lock (votingLock) { }
        }

        public List<Candidate> GetAllCandidates()
        {
            lock (candidatesLock)
            {
                return candidates.Values.Select(c => (Candidate)c.Clone()).ToList();
            }
        }

        public Candidate GetCandidateById(Guid id)
        {
            lock (candidatesLock)
            {
                if (candidates.TryGetValue(id, out var candidate))
                {
                    return (Candidate)candidate.Clone();
                }
                else
                {
                    throw new KeyNotFoundException("Candidate not found.");
                }
            }
        }

        public void Vote(Guid candidateId, string code)
        {
            lock (candidatesLock)
            {
                if (!availableCodes.Contains(code))
                {
                    throw new InvalidOperationException("Code is invalid or already used.");
                }

                foreach (var candidate in candidates.Values)
                {
                    if (candidate.Id == candidateId)
                    {
                        candidate.AddVotes(1);
                        availableCodes.Remove(code);
                        return;
                    }
                }

                throw new KeyNotFoundException("No candidate found.");
            }
        }

        private void AddCandidate(Candidate candidate)
        {
            lock (candidatesLock)
            {
                candidates.Add(candidate.Id, candidate);
            }
        }

        private async void SimulateVoting()
        {
            Random random = new Random();
            while (true)
            {
                int delay = random.Next(1000, 3000);
                await Task.Delay(delay);

                lock (votingLock)
                {
                    if (!isVotingActive)
                        break;
                }

                Candidate selectedCandidate;

                lock (candidatesLock)
                {
                    if (candidates.Count == 0)
                        continue;
                    var candidateList = candidates.Values.ToList();
                    selectedCandidate = candidateList[random.Next(candidateList.Count)];
                }

                int votesToAdd = random.Next(1, 11);
                lock (candidatesLock)
                {
                    if (candidates.TryGetValue(selectedCandidate.Id, out var candidate))
                    {
                        candidate.AddVotes(votesToAdd);
                    }
                }
            }
        }
    }
}
