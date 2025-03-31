using System;

namespace Data
{
    public class VotesChangeEventArgs : EventArgs
    {
        public VotesChangeEventArgs(Guid id, int votes)
        {
            Id = id;
            Votes = votes;
        }

        public Guid Id { get; }
        public int Votes { get; }
    }
}
