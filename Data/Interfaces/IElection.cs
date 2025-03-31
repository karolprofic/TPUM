using System;
using System.Collections.Generic;
using Data;

namespace Data.Interfaces
{
    public interface IElection
    {
        event EventHandler<VotesChangeEventArgs> VotesChange;
        List<Candidate> GetAllCandidates();
        Candidate GetCandidateById(Guid id);
        string GetElectionTitle();
        void Vote(Guid candidateId, string code);
        void SimulateVote();
    }

}
