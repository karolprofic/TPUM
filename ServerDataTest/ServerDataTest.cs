using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerData;
using ServerData.Interfaces;

namespace ServerDataTests
{
    [TestClass]
    public class ServerDataTest
    {
        private IElection election;

        [TestInitialize]
        public void Setup()
        {
            election = DataAbstractAPI.Create().GetElection();
        }

        [TestMethod]
        public void TestGetElectionTitleReturnsCorrectTitle()
        {
            string title = election.GetElectionTitle();
            Assert.AreEqual("Wybory Prezydenckie 2025", title);
        }

        [TestMethod]
        public void TestGetAllCandidatesReturnsClonedCandidates()
        {
            var candidates1 = election.GetAllCandidates();
            var candidates2 = election.GetAllCandidates();
            Assert.AreEqual(candidates1.Count, candidates2.Count);
            for (int i = 0; i < candidates1.Count; i++)
            {
                Assert.AreNotSame(candidates1[i], candidates2[i]);
            }
        }

        [TestMethod]
        public void TestGetCandidateByIdReturnsCorrectCandidate()
        {
            var candidates = election.GetAllCandidates();
            var candidateOriginal = candidates.First();
            Guid id = candidateOriginal.Id;
            var candidateRetrieved = election.GetCandidateById(id);
            Assert.AreEqual(candidateOriginal.Id, candidateRetrieved.Id);
            Assert.AreEqual(candidateOriginal.Name, candidateRetrieved.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestGetCandidateByIdThrowsExceptionForInvalidId()
        {
            election.GetCandidateById(Guid.NewGuid());
        }

        [TestMethod]
        public void TestVoteIncrementsVoteWithValidCode()
        {
            var candidate = election.GetAllCandidates().First();
            int initialVotes = candidate.Votes;
            string validCode = "123456";
            election.Vote(candidate.Id, validCode);
            var candidateAfter = election.GetCandidateById(candidate.Id);
            Assert.AreEqual(initialVotes + 1, candidateAfter.Votes);
        }

        [TestMethod]
        public void TestVoteDoesNothingWithInvalidOrUsedCode()
        {
            var candidate = election.GetAllCandidates().First();
            int initialVotes = candidate.Votes;
            string validCode = "234567";
            election.Vote(candidate.Id, validCode);
            election.Vote(candidate.Id, validCode);
            var candidateAfter = election.GetCandidateById(candidate.Id);
            Assert.AreEqual(initialVotes + 1, candidateAfter.Votes);
        }

        [TestMethod]
        public void TestSimulateVoteIncreasesVotesForAllCandidates()
        {
            var candidatesBefore = election.GetAllCandidates();
            int totalVotesBefore = candidatesBefore.Sum(c => c.Votes);
            election.SimulateVote();
            var candidatesAfter = election.GetAllCandidates();
            int totalVotesAfter = candidatesAfter.Sum(c => c.Votes);
            Assert.IsTrue(totalVotesAfter > totalVotesBefore);
        }
    }
}