using ServerLogic;
using ServerLogic.Interfaces;
using ServerData;
using ServerData.Interfaces;

namespace ServerLogicTest
{
    [TestClass]
    public class ServerLogicTest
    {
        private IElectionSystem electionSystem = null!;

        [TestInitialize]
        public void Setup()
        {
            electionSystem = LogicAbstractAPI.Create().GetElectionSystem();
        }

        [TestMethod]
        public void GetElectionTitleShouldReturnCorrectTitle()
        {
            Assert.AreEqual("Wybory Prezydenckie 2025", electionSystem.GetElectionTitle());
        }

        [TestMethod]
        public void GetCandidatesShouldReturnEmptyList()
        {
            var candidates = electionSystem.GetCandidates();
            Assert.IsNotNull(candidates);
            Assert.AreEqual(8, candidates.Count);
        }

        [TestMethod]
        public void CastInvalidVoteShouldNotThrowAndNotChangeCandidates()
        {
            var candidateId = Guid.NewGuid();
            electionSystem.CastVote(candidateId, "999999");
            var candidatesAfter = electionSystem.GetCandidates();
            Assert.AreEqual(8, candidatesAfter.Count);
        }
    }
}
