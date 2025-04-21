using ServerLogic.Interfaces;
using ServerData.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Commons;

namespace ServerLogic
{
    public class ElectionSystem : IElectionSystem
    {
        private readonly IElection _election;
        private readonly List<IObserver<List<CandidateDTO>>> _observers = new();
        private bool _simulationActive = true;

        public ElectionSystem(IElection election)
        {
            _election = election;
            SimulateVoting();
        }

        ~ElectionSystem()
        {
            _simulationActive = false;
        }

        public string GetElectionTitle() => _election.GetElectionTitle();

        public List<CandidateDTO> GetCandidates()
        {
            return _election.GetAllCandidates().ConvertAll(c => new CandidateDTO
            {
                Id = c.Id,
                Name = c.Name,
                Surname = c.Surname,
                Votes = c.Votes
            });
        }

        public void CastVote(Guid candidateId, string code)
        {
            _election.Vote(candidateId, code);
            NotifyObservers();
        }

        private async void SimulateVoting()
        {
            var rnd = new Random();
            while (_simulationActive)
            {
                await Task.Delay(rnd.Next(2000, 6000));
                _election.SimulateVote();
                NotifyObservers();
            }
        }

        private void NotifyObservers()
        {
            var snapshot = GetCandidates();
            foreach (var obs in _observers)
            {
                obs.OnNext(snapshot);
            }
        }

        public IDisposable Subscribe(IObserver<List<CandidateDTO>> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            observer.OnNext(GetCandidates());
            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<List<CandidateDTO>>> _obs;
            private readonly IObserver<List<CandidateDTO>> _observer;

            public Unsubscriber(List<IObserver<List<CandidateDTO>>> obs, IObserver<List<CandidateDTO>> observer)
            {
                _obs = obs;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_obs.Contains(_observer))
                    _obs.Remove(_observer);
            }
        }
    }
}