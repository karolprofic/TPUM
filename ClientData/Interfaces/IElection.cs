using System;
using System.Collections.Generic;

namespace Data.Interfaces
{
    public interface IElection
    {
        event EventHandler<VotesChangeEventArgs> VotesChange;
        List<ICandidate> GetAllCandidates();
        string GetElectionTitle();
        void Vote(Guid candidateId, string code);
    }

}
