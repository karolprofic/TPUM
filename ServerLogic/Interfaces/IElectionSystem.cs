using System.Collections.Generic;
using System;
using Commons;

namespace ServerLogic.Interfaces
{
    public interface IElectionSystem : IObservable<List<CandidateDTO>>
    {
        string GetElectionTitle();
        List<CandidateDTO> GetCandidates();
        void CastVote(Guid candidateId, string code);
    }
}
