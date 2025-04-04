using System.Collections.Generic;
using System;

namespace Logic.Interfaces
{
    public interface IElectionSystem
    {
        event EventHandler<VotesChangeEventArgs> VotesChange;
        string GetElectionTitle();
        List<CandidateDTO> GetCandidates();
        void CastVote(Guid candidateId, string code);
    }
}
