﻿
using Data;
using Data.Interfaces;
using Logic.Interfaces;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public class ElectionSystem : IElectionSystem
    {
        public event EventHandler<VotesChangeEventArgs> VotesChange;
        private readonly IElection election;
        private bool votingSimulationActive = true;

        public ElectionSystem(IElection election)
        {
            this.election = election;
            this.election.VotesChange += OnVotesChanged;
            SimulateVoting();
        }

        ~ElectionSystem()
        {
            votingSimulationActive = false;
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

        private async void SimulateVoting()
        {
            Random random = new Random();
            while (true)
            {
                int delay = random.Next(1000, 3000);
                await Task.Delay(delay);

                if (!votingSimulationActive)
                    break;

                election.SimulateVote();
            }
        }

        private void OnVotesChanged(object sender, Data.VotesChangeEventArgs e)
        {
            EventHandler<VotesChangeEventArgs> handler = VotesChange;
            handler?.Invoke(this, new Logic.VotesChangeEventArgs(e.Id, e.Votes));
        }
    }
}
