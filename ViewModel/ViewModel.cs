using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ViewModel
{
    public class CandidateViewModel : INotifyPropertyChanged
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        private int votes;
        public int Votes
        {
            get => votes;
            set
            {
                votes = value;
                OnPropertyChanged(nameof(Votes));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ViewModel : INotifyPropertyChanged
    {
        private string keyAccess;
        public string KeyAccess
        {
            get => keyAccess;
            set
            {
                if (keyAccess != value)
                {
                    keyAccess = value;
                    OnPropertyChanged(nameof(KeyAccess));
                }
            }
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

        public ObservableCollection<CandidateViewModel> Candidates { get; } = new ObservableCollection<CandidateViewModel>();

        public ICommand VoteCommand { get; }

        public ViewModel()
        {
            electionTitle = "Wybory Prezydenckie 2025";
            keyAccess = "";
            Candidates.Add(new CandidateViewModel { Id = Guid.NewGuid(), Name = "Jan", Surname = "Kowalski", Votes = 0 });
            Candidates.Add(new CandidateViewModel { Id = Guid.NewGuid(), Name = "Anna", Surname = "Nowak", Votes = 0 });
            Candidates.Add(new CandidateViewModel { Id = Guid.NewGuid(), Name = "Piotr", Surname = "Wiœniewski", Votes = 0 });

            VoteCommand = new RelayCommand(Vote);
        }

        private void Vote(object parameter)
        {
            if (parameter is Guid candidateId)
            {
                var candidate = Candidates.FirstOrDefault(c => c.Id == candidateId);
                if (candidate != null)
                {
                    if (keyAccess != null && keyAccess.Equals("111111"))
                    {
                        candidate.Votes++;
                    }
                    OnPropertyChanged(nameof(Candidates));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
