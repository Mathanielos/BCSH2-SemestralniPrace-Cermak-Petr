using Avalonia.Media.Imaging;
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
        private ObservableCollection<Country> showingCountry;
        private Bitmap image;
        private ObservableCollection<City> cities;
        public ObservableCollection<Country> ShowingCountry
        {
            get => showingCountry;
            private set => this.RaiseAndSetIfChanged(ref showingCountry, value);
        }
        public Bitmap Image
        {
            get => image;
            private set => this.RaiseAndSetIfChanged(ref image, value);
        }
        public ObservableCollection<City> Cities
        {
            get => cities;
            set => this.RaiseAndSetIfChanged(ref cities, value);
        }
        public CountryViewModel(Country country)
        {
            ShowingCountry = new ObservableCollection<Country>
            {
                country
            };
            Image = country.Image;
            cities = new ObservableCollection<City>(country.Cities);
        }
        public override void RemovePlace(Place place)
        {
            // Tries to find the city containing the place
            City cityContainingPlace = Cities.FirstOrDefault(city => city.Places.Contains(place));

            if (cityContainingPlace != null && cityContainingPlace.Places.Remove(place))
            {
                Parent?.RemovePlace(place);
            }
        }
        public override void RemoveCity(City city)
        {
            if (Cities.Remove(city))
            {
                Parent?.RemoveCity(city);
            }
        }
        public override void UpdatePlace(Place place) // Changes the place based on Id
        {
            // Tries to find the city containing the place
            int indexCity = Cities.IndexOf(Cities.FirstOrDefault(city => city.Places.Contains(place)));

            if (indexCity != -1)
            {
                int indexPlace = Cities[indexCity].Places.IndexOf(Cities[indexCity].Places.FirstOrDefault(p => p.Id == place.Id));
                if (indexPlace != -1)
                {
                    Cities[indexCity].Places[indexPlace] = place;
                    Parent?.UpdatePlace(place);
                }
            }
        }
        public override void UpdateCity(City city) // Changes the city based on Id
        {
            int index = Cities.IndexOf(Cities.FirstOrDefault(c => c.Id == city.Id));

            if (index != -1)
            {
                Cities[index] = city;
                Parent?.UpdateCity(city);
            }
        }
        public override void UpdateCountry(Country country) // Changes the country
        {
            ShowingCountry[0] = country;
            Image = country.Image;
            Parent?.UpdateCountry(country);
        }
        public override void InsertPlace(Place place, int parentId) // Inserts new place
        {
            // Tries to find the city containing the place
            City cityContainingPlace = ShowingCountry[0].Cities.FirstOrDefault(city => city.Id == parentId);

            if (cityContainingPlace != null)
            {
                cityContainingPlace.Places.Add(place);
                Parent?.InsertPlace(place, parentId);
            }
        }
        public override void InsertCity(City city, int parentId) // Inserts new city
        {
            Cities.Add(city);
            Parent?.InsertCity(city, parentId);
        }
    }
}
