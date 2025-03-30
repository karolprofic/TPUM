using Data;
using Data.Interfaces;
using Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CandidatePresentation : INotifyPropertyChanged
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public int Votes { get; private set; }


        public CandidatePresentation(CandidateDTO candidate)
        {
            Id = candidate.Id;
            Name = candidate.Name;
            Surname = candidate.Surname;
            Votes = candidate.Votes;
        }

        public CandidatePresentation(Guid id, string name, string surname, int votes)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Votes = votes;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
