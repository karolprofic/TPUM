using System;
using System.Collections.Generic;
using System.Text;
using Data.Interfaces;
using static System.Collections.Specialized.BitVector32;

namespace Data
{
    internal class DataAPI : DataAbstractAPI
    {
        private readonly Election election;

        public DataAPI()
        {
            election = new Election();
        }

        public override IElection GetElection()
        {
            return election;
        }

    }
}