using Data;
using Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public class LogicAPI : LogicAbstractAPI
    {
        private readonly IElectionSystem electionSystem;

        public LogicAPI(DataAbstractAPI dataAPI) : base(dataAPI)
        {
            this.electionSystem = new ElectionSystem(dataAPI.GetElection());
        }

        public override IElectionSystem GetElectionSystem()
        {
            return electionSystem;
        }
    }
}
