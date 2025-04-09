using System;
using Data.Interfaces;

namespace Data
{
    public class Candidate : ICandidate, ICloneable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Votes { get; set; }

        public Candidate(Guid id, string name, string surname, int votes)
        {
            Id = id;
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

        public override string ToString()
        {
            return $"{Name} {Surname} (Votes: {Votes}, Id: {Id})";
        }
    }
}
