using System;

namespace Data.Interfaces
{
    public interface ICandidate : ICloneable
    {
        Guid Id { get; }
        string Name { get; }
        string Surname { get; }
        int Votes { get; }
    }
}
