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
        private IElectionSystem electionSystem { get; set; }

        public ElectionPresentation(IElectionSystem electionSystem)
        {
            this.electionSystem = electionSystem;
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
