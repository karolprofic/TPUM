using System;

namespace ServerData.Interfaces
{
    public interface ICandidate : ICloneable
    {
        Guid Id { get; }
        string Name { get; }
        string Surname { get; }
        int Votes { get; }
        void AddVotes(int count);
    }
}
