using ServerData;
using ServerLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerLogic
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
