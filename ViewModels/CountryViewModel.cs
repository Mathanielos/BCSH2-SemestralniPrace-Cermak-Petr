using BCSH2SemestralniPraceCermakPetr.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2SemestralniPraceCermakPetr.ViewModels
{
    public class CountryViewModel : ViewModelBase
    {
        private Country showingCountry;
        private ObservableCollection<City> cities;
        public Country ShowingCountry
        {
            get => showingCountry;
            private set => this.RaiseAndSetIfChanged(ref showingCountry, value);
        }
        public ObservableCollection<City> Cities
        {
            get => cities;
            set => this.RaiseAndSetIfChanged(ref cities, value);
        }
        public CountryViewModel(Country country)
        {
            ShowingCountry = country;
            cities = new ObservableCollection<City>(country.Cities);
        }
        public override void RemovePlace(Place place)
        {
            // Try to find the city containing the place
            City cityContainingPlace = ShowingCountry.Cities.FirstOrDefault(city => city.Places.Contains(place));

            if (cityContainingPlace != null && cityContainingPlace.Places.Remove(place))
            {
                this.RaisePropertyChanged(nameof(ShowingCountry));
            }

            Parent?.RemovePlace(place);
        }
        public override void RemoveCity(City city)
        {
            if (Cities.Remove(city))
            {
                this.RaisePropertyChanged(nameof(Cities));
            }

            Parent?.RemoveCity(city);
        }

    }
}
