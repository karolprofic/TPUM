using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Data;
using Data.Interfaces;
using static System.Collections.Specialized.BitVector32;

namespace DataTest
{
    [TestClass]
    public class DataTest
    {
        [TestMethod]
        public void CandidateCloneTest()
        {
            var candidate = new Candidate("Test", "Candidate", 5);
            var candidateClone = (Candidate)candidate.Clone();
            Assert.AreNotSame(candidate, candidateClone);
            Assert.AreNotSame(candidate.Name, candidateClone.Name);
            Assert.AreNotSame(candidate.Surname, candidateClone.Surname);
            Assert.AreEqual(candidate.Name, candidateClone.Name);
            Assert.AreEqual(candidate.Surname, candidateClone.Surname);
            Assert.AreEqual(candidate.Votes, candidateClone.Votes);
            Assert.AreEqual(candidate.Id, candidateClone.Id);
        }

        [TestMethod]
        public void CandidateAddVotesTest()
        {
            var candidate = new Candidate("Test", "Candidate", 0);
            candidate.AddVotes(3);
            Assert.AreEqual(3, candidate.Votes);
            candidate.AddVotes(-5);
            Assert.AreEqual(3, candidate.Votes);
        }

        [TestMethod]
        public void ElectionCandidatesCountTest()
        {
            IElection election = DataAbstractAPI.Create().GetElection();
            var candidates = election.GetAllCandidates();
            Assert.AreEqual(8, candidates.Count, "The initial number of candidates should be 8.");
        }

        [TestMethod]
        public void ElectionVoteTest()
        {
            IElection election = DataAbstractAPI.Create().GetElection();
            var candidates = election.GetAllCandidates();
            var candidate = candidates.First();
            int baselineVotes = candidate.Votes;
            string validCode = "123456";
            election.Vote(candidate.Id, validCode);
            var updatedCandidate = election.GetCandidateById(candidate.Id);
            Assert.IsTrue(updatedCandidate.Votes >= baselineVotes + 1, "The number of votes of a candidate should increase by at least 1.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ElectionVoteInvalidCodeTest()
        {
            IElection election = DataAbstractAPI.Create().GetElection();
            var candidate = election.GetAllCandidates().First();
            election.Vote(candidate.Id, "invalidCode");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void ElectionVoteInvalidCandidateTest()
        {
            IElection election = DataAbstractAPI.Create().GetElection();
            election.Vote(Guid.NewGuid(), "234567");
        }

        [TestMethod]
        public void CandidateToStringReturnsCorrectFormat()
        {
            var candidate = new Candidate("John", "Doe", 5);
            string expected = $"John Doe (Votes: 5, Id: {candidate.Id})";
            string result = candidate.ToString();
            Assert.AreEqual(expected, result);
        }

    }
}
