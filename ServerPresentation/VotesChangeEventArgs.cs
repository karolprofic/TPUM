using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class VotesChangeEventArgs : EventArgs
    {
        public VotesChangeEventArgs(Guid id, int votes) {}
    }
}
