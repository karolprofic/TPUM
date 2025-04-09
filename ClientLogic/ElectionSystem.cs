
using Data;
using Data.Interfaces;
using Logic.Interfaces;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    internal class ElectionSystem : IElectionSystem
    {
        public event EventHandler<VotesChangeEventArgs> VotesChange;
        private readonly IElection election;

        public ElectionSystem(IElection election)
        {
            this.election = election;
            this.election.VotesChange += OnVotesChanged;
        }

        public string GetElectionTitle()
        {
            return election.GetElectionTitle();
        }

        public List<CandidateDTO> GetCandidates()
        {
            List<CandidateDTO> candidateDTOs = election.GetAllCandidates().Select(c => new CandidateDTO
            {
                Id = c.Id,
                Name = c.Name,
                Surname = c.Surname,
                Votes = c.Votes
            }).ToList();
            return candidateDTOs;
        }

        public void CastVote(Guid candidateId, string code)
        {
            election.Vote(candidateId, code); 
        }

        private void OnVotesChanged(object sender, Data.VotesChangeEventArgs e)
        {
            EventHandler<VotesChangeEventArgs> handler = VotesChange;
            handler?.Invoke(this, new Logic.VotesChangeEventArgs(e.Id, e.Votes));
        }
    }
}
