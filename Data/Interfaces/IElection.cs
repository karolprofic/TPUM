using System;
using System.Collections.Generic;
using Data;

namespace Data.Interfaces
{
    public interface IElection
    {
        List<Candidate> GetAllCandidates();
        Candidate GetCandidateById(Guid id);
        string GetElectionTitle();
        void Vote(Guid candidateId, string code);
        void SimulateVote();
    }

}
