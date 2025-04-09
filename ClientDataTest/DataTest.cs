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
            var candidate = new Candidate(Guid.NewGuid(), "Jan", "Kowalski", 10);
            Assert.AreEqual("Jan", candidate.Name);
            Assert.AreEqual("Kowalski", candidate.Surname);
            Assert.AreEqual(10, candidate.Votes);
            Assert.AreNotEqual(Guid.Empty, candidate.Id);
        }

        [TestMethod]
        public void VotesShouldBeExaclyAsSetIncreaseVoteCount()
        {
            var candidate = new Candidate(Guid.NewGuid(), "Anna", "Nowak", 5);
            Assert.AreEqual(5, candidate.Votes);
        }

        [TestMethod]
        public void CloneShouldCreateCopy()
        {
            var candidate = new Candidate(Guid.NewGuid(),"Maria", "Wiœniewska", 7);
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
            var candidate = new Candidate(Guid.NewGuid(),"Tomasz", "Zieliñski", 12);
            string expected = $"Tomasz Zieliñski (Votes: 12, Id: {candidate.Id})";
            Assert.AreEqual(expected, candidate.ToString());
        }

        [TestMethod]
        public void GetElectionTitleShouldReturnCorrectTitle()
        {
            IElection election = DataAbstractAPI.Create().GetElection();
            Assert.AreEqual("£adowanie...", election.GetElectionTitle());
        }

        [TestMethod]
        public void GetAllCandidatesShouldReturnEmptyList()
        {
            IElection election = DataAbstractAPI.Create().GetElection();
            var candidates = election.GetAllCandidates();
            Assert.IsTrue(candidates.Count == 0);
        }

    }
}
