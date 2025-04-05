using System;
using System.Collections.Generic;
using System.Text;

namespace Model
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
