using Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ViewModel
{

    public class ViewModel : INotifyPropertyChanged
    {
        private string keyAccess = string.Empty;
        public string KeyAccess
        {
            get => keyAccess;
            set => SetProperty(ref keyAccess, value);
        }

        private string electionTitle;
        public string ElectionTitle
        {
            get => electionTitle;
            set
            {
                if (electionTitle != value)
                {
                    electionTitle = value;
                    OnPropertyChanged(nameof(ElectionTitle));
                }
            }
        }

        private ObservableCollection<CandidatePresentation> candidates = [];
        public ObservableCollection<CandidatePresentation> Candidates
        {
            get => candidates;
            private set
            {
                if (candidates != value)
                {
                    candidates = value;
                    OnPropertyChanged(nameof(Candidates));
                }
            }
        }


        public ICommand VoteCommand { get; }

        private Model.Model model;

        public ViewModel()
        {
            this.model = new Model.Model(null!);
            electionTitle = model.electionPresentation.GetElectionTitle();
            keyAccess = string.Empty;
            Candidates = new ObservableCollection<CandidatePresentation>(model.electionPresentation.GetCandidates());
            model.electionPresentation.VotesChanged += OnVotesChanged;

            VoteCommand = new RelayCommand(Vote);
        }

        private void OnVotesChanged(object? sender, VotesChangeEventArgs e)
        {
            RefreshView();
        }

        private void Vote(object parameter)
        {
            if (parameter is Guid candidateId)
            {
                var candidate = Candidates.FirstOrDefault(c => c.Id == candidateId);
                if (candidate != null)
                {
                    model.Vote(candidateId, keyAccess);
                    RefreshView();
                    OnPropertyChanged(nameof(Candidates));
                }
            }
        }

        private void RefreshView()
        {
            ElectionTitle = model.electionPresentation.GetElectionTitle();
            Candidates = new ObservableCollection<CandidatePresentation>(model.electionPresentation.GetCandidates());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null!)
        {
            if (!Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
    }
}
