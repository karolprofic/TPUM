using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logic;
using Logic.Interfaces;

namespace LogicTest
{
    [TestClass]
    public class LogicTest
    {
        private IElectionSystem electionSystem = null!;

        [TestInitialize]
        public void Setup()
        {
            electionSystem = LogicAbstractAPI.Create(new DataMock()).GetElectionSystem();
        }

        [TestMethod]
        public void GetElectionTitle_ShouldReturnCorrectTitle()
        {
            Assert.AreEqual("Wybory Testowe", electionSystem.GetElectionTitle());
        }

        [TestMethod]
        public void GetCandidatesShouldReturnCandidateDTOList()
        {
            var candidates = electionSystem.GetCandidates();
            Assert.IsNotNull(candidates);
            Assert.IsTrue(candidates.Count > 0);
            var candidate = candidates.First();
            Assert.IsFalse(string.IsNullOrWhiteSpace(candidate.Name));
            Assert.IsFalse(string.IsNullOrWhiteSpace(candidate.Surname));
            Assert.AreEqual("Jan", candidate.Name);
            Assert.AreEqual("Kowalski", candidate.Surname);
        }

        [TestMethod]
        public void CastVoteWithValidCodeShouldIncreaseCandidateVotes()
        {
            var candidateDTO = electionSystem.GetCandidates().First();
            int initialVotes = candidateDTO.Votes;
            electionSystem.CastVote(candidateDTO.Id, "123456");
            var updatedCandidate = electionSystem.GetCandidates().First(c => c.Id == candidateDTO.Id);
            Assert.IsTrue(updatedCandidate.Votes == initialVotes + 1);
        }

        // TODO: Remove + test or throw exceptions for simulate voting and this one
        // TOOD: Use CandidateDTO instate of Candidate
        [TestMethod]
        public async Task VotesChangeEventShouldBeTriggeredOnVote()
        {
            var candidateDTO = electionSystem.GetCandidates().First();
            bool eventTriggered = false;
            electionSystem.VotesChange += (sender, e) =>
            {
                if (e.Id == candidateDTO.Id)
                    eventTriggered = true;
            };

            electionSystem.CastVote(candidateDTO.Id, "234567");
            await Task.Delay(500);
            Assert.IsTrue(eventTriggered, "Zdarzenie VotesChange nie zostało wywołane.");
        }
    }
}
