
using Logic;

namespace Model
{
    public class Model
    {
        private LogicAbstractAPI logicAbstractAPI;

        public ElectionPresentation electionPresentation { get; private set; }

        public Model(LogicAbstractAPI? logicAbstractAPI)
        {
            this.logicAbstractAPI = logicAbstractAPI ?? LogicAbstractAPI.Create();
            this.electionPresentation = new ElectionPresentation(this.logicAbstractAPI.GetElectionSystem());
        }

        public void Vote(Guid candidateId, string code)
        {
            logicAbstractAPI.GetElectionSystem().CastVote(candidateId, code);
        }


    }

}
