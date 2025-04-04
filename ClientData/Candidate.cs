using System;
using Data.Interfaces;

namespace Data
{
    public class Candidate : ICandidate, ICloneable
    {
        public Guid Id { get; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public int Votes { get; private set; }

        public Candidate(string name, string surname, int votes)
        {
            Id = Guid.NewGuid();
            Name = name;
            Surname = surname;
            Votes = votes;
        }

        public object Clone()
        {
            Candidate clone = (Candidate)MemberwiseClone();
            clone.Name = string.Copy(Name);
            clone.Surname = string.Copy(Surname);
            return clone;
        }

        public void AddVotes(int count)
        {
            if (count > 0)
            {
                Votes += count;
            }
        }

        public override string ToString()
        {
            return $"{Name} {Surname} (Votes: {Votes}, Id: {Id})";
        }
    }
}
