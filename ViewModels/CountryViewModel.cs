using BCSH2SemestralniPraceCermakPetr.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2SemestralniPraceCermakPetr.ViewModels
{
    public class CountryViewModel : ViewModelBase
    {
        private Country showingCountry;
        private string countryName;
        public Country ShowingCountry
        {
            get => showingCountry;
            private set => this.RaiseAndSetIfChanged(ref showingCountry, value);
        }
        public string CountryName
        {
            get => countryName;
            private set => this.RaiseAndSetIfChanged(ref countryName, value);
        }

        public CountryViewModel(string country)
        {
            //ShowingCountry = country;
            CountryName = country;
        }
    }
}
