using System;
using System.Collections.Generic;

namespace ServerData.Interfaces
{
    public interface IElection
    {
        List<ICandidate> GetAllCandidates();
        ICandidate GetCandidateById(Guid id);
        string GetElectionTitle();
        void Vote(Guid candidateId, string code);
        void SimulateVote();
    }

}
