using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Commons;
using Data;
using Data.Interfaces;
using ServerPresentation;

namespace ServerClientTest
{
    [TestClass]
    public class ServerClientTest
    {
        private const int MaxRetry = 100;
        private const int RetryDelayMs = 1000;

        private async Task WaitUntil(Func<bool> condition, int maxRetry = MaxRetry)
        {
            for (int i = 0; i < maxRetry; i++)
            {
                await Task.Delay(RetryDelayMs);
                if (condition())
                {
                    return;
                }
            }
            Assert.Fail("Condition not met within retry limit.");
        }

        [TestMethod]
        public async Task ClientReceivesCandidatesAndCastVote()
        {
            // Server -> Client
            ElectionServer server = new ElectionServer();
            var serverTask = Task.Run(() => server.StartAsync());
            var clientData = DataAbstractAPI.Create().GetElection();

            await WaitUntil(() =>
            {
                var candidatesList = clientData.GetAllCandidates();
                return candidatesList != null && candidatesList.Any();
            });

            List<ICandidate> receivedCandidates = clientData.GetAllCandidates();
            Assert.IsTrue(receivedCandidates.Count == 8, "Candidates list should contain 8 candidates");

            // Client -> Server
            var candidate = receivedCandidates[0];
            var candidateId = candidate.Id;
            int candidateVotes = candidate.Votes;
            clientData.Vote(candidate.Id, "1234567");

            await WaitUntil(() =>
            {
                var updated = clientData.GetAllCandidates().FirstOrDefault(c => c.Id == candidateId);
                return updated != null && updated.Votes > candidateVotes;
            });

            var updatedCandidate = clientData.GetAllCandidates().First(c => c.Id == candidateId);
            Assert.IsTrue(updatedCandidate.Votes > candidateVotes, "Vote count should increase after voting");

        }
    }
}