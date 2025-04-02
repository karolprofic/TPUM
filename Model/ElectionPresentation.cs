using Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ElectionPresentation
    {
        public event EventHandler<VotesChangeEventArgs> VotesChanged;

        private IElectionSystem electionSystem { get; set; }

        public ElectionPresentation(IElectionSystem electionSystem)
        {
            this.electionSystem = electionSystem;
            this.electionSystem.VotesChange += OnVotesChanged;
        }

        private void OnVotesChanged(object? sender, Logic.VotesChangeEventArgs e)
        {
            EventHandler<VotesChangeEventArgs> handler = VotesChanged;
            handler?.Invoke(this, new VotesChangeEventArgs(e.Id, e.Votes));
        }

        public List<CandidatePresentation> GetCandidates()
        {
            return electionSystem.GetCandidates()
                .Select(item => new CandidatePresentation(item))
                .Cast<CandidatePresentation>()
                .ToList();
        }

        public string GetElectionTitle()
        {
            return electionSystem.GetElectionTitle();
        }

    }
}
