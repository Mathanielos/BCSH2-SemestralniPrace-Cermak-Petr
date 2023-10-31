using BCSH2SemestralniPraceCermakPetr.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2SemestralniPraceCermakPetr.ViewModels
{
    public class CityViewModel : ViewModelBase
    {
        private City showingCity;
        public City ShowingCity
        {
            get => showingCity;
            private set => this.RaiseAndSetIfChanged(ref showingCity, value);
        }
        public CityViewModel(City city)
        {
            ShowingCity = city;
        }
    }
}
