using Data;
using Data.Interfaces;

namespace DataTest
{
    [TestClass]
    public class DataTest
    {
        [TestMethod]
        public void CandidateShouldInitializeCorrectly()
        {
            var candidate = new Candidate("Jan", "Kowalski", 10);
            Assert.AreEqual("Jan", candidate.Name);
            Assert.AreEqual("Kowalski", candidate.Surname);
            Assert.AreEqual(10, candidate.Votes);
            Assert.AreNotEqual(Guid.Empty, candidate.Id);
        }

        [TestMethod]
        public void AddVotesShouldIncreaseVoteCount()
        {
            var candidate = new Candidate("Anna", "Nowak", 5);
            candidate.AddVotes(3);
            Assert.AreEqual(8, candidate.Votes);
        }

        [TestMethod]
        public void AddVotesShouldNotDecreaseVoteCount()
        {
            var candidate = new Candidate("Piotr", "Wiœniewski", 5);
            candidate.AddVotes(-3);
            Assert.AreEqual(5, candidate.Votes);
        }

        [TestMethod]
        public void CloneShouldCreateCopy()
        {
            var candidate = new Candidate("Maria", "Wiœniewska", 7);
            var clone = (Candidate)candidate.Clone();
            Assert.AreNotSame(candidate, clone);
            Assert.AreNotSame(candidate.Name, clone.Name);
            Assert.AreNotSame(candidate.Surname, clone.Surname);
            Assert.AreEqual(candidate.Name, clone.Name);
            Assert.AreEqual(candidate.Surname, clone.Surname);
            Assert.AreEqual(candidate.Votes, clone.Votes);
            Assert.AreEqual(candidate.Id, clone.Id);
        }

        [TestMethod]
        public void ToStringShouldReturnFormattedString()
        {
            var candidate = new Candidate("Tomasz", "Zieliñski", 12);
            string expected = $"Tomasz Zieliñski (Votes: 12, Id: {candidate.Id})";
            Assert.AreEqual(expected, candidate.ToString());
        }

        [TestMethod]
        public void GetElectionTitleShouldReturnCorrectTitle()
        {
            IElection election = DataAbstractAPI.Create().GetElection();
            Assert.AreEqual("Wybory Prezydenckie 2025", election.GetElectionTitle());
        }

        [TestMethod]
        public void GetAllCandidatesShouldReturnNonEmptyList()
        {
            IElection election = DataAbstractAPI.Create().GetElection();
            var candidates = election.GetAllCandidates();
            Assert.IsTrue(candidates.Count > 0);
        }

        [TestMethod]
        public void GetCandidateByIdShouldReturnCorrectCandidate()
        {
            IElection election = DataAbstractAPI.Create().GetElection();
            var candidate = election.GetAllCandidates().First();
            var retrievedCandidate = election.GetCandidateById(candidate.Id);
            Assert.AreEqual(candidate.Name, retrievedCandidate.Name);
            Assert.AreEqual(candidate.Surname, retrievedCandidate.Surname);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetCandidateByInvalidIdShouldThrowException()
        {
            IElection election = DataAbstractAPI.Create().GetElection();
            election.GetCandidateById(Guid.NewGuid());
        }

        [TestMethod]
        public void VoteValidCodeShouldIncreaseVoteCount()
        {
            IElection election = DataAbstractAPI.Create().GetElection();
            var candidate = election.GetAllCandidates().First();
            election.Vote(candidate.Id, "123456");
            var updatedCandidate = election.GetCandidateById(candidate.Id);
            Assert.AreEqual(1, updatedCandidate.Votes);
        }

        [TestMethod]
        public void VoteInvalidCodeShouldNotIncreaseVoteCount()
        {
            IElection election = DataAbstractAPI.Create().GetElection();
            var candidate = election.GetAllCandidates().First();
            election.Vote(candidate.Id, "999999");
            var updatedCandidate = election.GetCandidateById(candidate.Id);
            Assert.AreEqual(0, updatedCandidate.Votes);
        }

        [TestMethod]
        public void SimulateVoteShouldIncreaseVotesForAllCandidates()
        {
            IElection election = DataAbstractAPI.Create().GetElection();
            var initialVotes = election.GetAllCandidates().Sum(c => c.Votes);
            election.SimulateVote();
            var updatedVotes = election.GetAllCandidates().Sum(c => c.Votes);
            Assert.IsTrue(updatedVotes > initialVotes);
        }

    }
}
