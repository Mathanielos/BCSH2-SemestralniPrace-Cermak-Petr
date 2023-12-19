using BCSH2SemestralniPraceCermakPetr.Models;
using BCSH2SemestralniPraceCermakPetr.Models.Services;
using ReactiveUI;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Numerics;
using System.IO;
using Avalonia.Media.Imaging;
using BCSH2SemestralniPraceCermakPetr.Models.Enums;
using System.Reflection.Metadata;
using System.Linq;
using System.Reactive;

namespace BCSH2SemestralniPraceCermakPetr.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly DatabaseService databaseService;
        private ObservableCollection<Country> countries;

        public ReactiveCommand<Unit, Unit> RestoreCommand { get; }

        public ObservableCollection<Country> Countries
        {
            get => countries;
            set => this.RaiseAndSetIfChanged(ref countries, value);
        }
        public MainViewModel(DatabaseService service)
        {
            databaseService = service;
            RestoreCommand = ReactiveCommand.Create(Restore);
            LoadCountries();
        }

        private void LoadCountries()
        {
            // Fetch data for all countries
            List<Dictionary<string, object>> countryData = databaseService.GetData("Countries", null);

            Countries = new ObservableCollection<Country>();

            if (countryData.Count > 0)
            {
                foreach (var item in countryData)
                {
                    // Create a Country object from the retrieved data
                    Country country = new(
                        Convert.ToInt32(item["CountryID"]),
                        item["Name"] as string,
                        item["Description"] as string,
                        item["Tips"] as string,
                        new List<City>(),
                        ConvertByteArrayToBitmap(item["Image"] as byte[])
                    );

                    // Fetch city data for the specific country from the database
                    List<Dictionary<string, object>> cityData = databaseService.GetData("Cities", null, "CountryID = @CountryID", new Dictionary<string, object> { { "CountryID", item["CountryID"] } });

                    foreach (var cityAttributes in cityData)
                    {
                        // Create a City object for each city
                        City city = new City(
                            Convert.ToInt32(cityAttributes["CityID"]),
                            cityAttributes["Name"] as string,
                            cityAttributes["Description"] as string,
                            cityAttributes["BasicInformation"] as string,
                            new List<Place>(), // Initialize an empty list of places for the city
                            ConvertByteArrayToBitmap(cityAttributes["Image"] as byte[])
                        );

                        // Fetch place data for the specific city from the database
                        List<Dictionary<string, object>> placeData = databaseService.GetData("Places", null, "CityID = @CityID", new Dictionary<string, object> { { "CityID", cityAttributes["CityID"] } });

                        foreach (var placeAttributes in placeData)
                        {
                            // Create a Place object for each place
                            Place place = new Place(
                                Convert.ToInt32(placeAttributes["PlaceID"]),
                                placeAttributes["Name"] as string,
                                placeAttributes["Description"] as string,
                                ConvertByteArrayToBitmap(placeAttributes["Image"] as byte[]),
                                (Category)Enum.Parse(typeof(Category), placeAttributes["CategoryID"].ToString())
                            );

                            // Add the Place object to the list of places in the City
                            city.Places.Add(place);
                        }

                        // Add the City object to the list of cities in the Country
                        country.Cities.Add(city);
                    }
                    Countries.Add(country);
                }
            }
        }
        private void Restore()
        {
            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Restore from TipsToTravelDefault.db
            var defaultDbPath = Path.Combine(appDirectory, "Assets/TipsToTravelDefault.db");
            databaseService.DatabaseFile = defaultDbPath;
            databaseService.ConnectionString = $"Data Source={defaultDbPath}";
            LoadCountries();

            // Replace TipsToTravelChanges.db with TipsToTravelDefault.db
            var changesDbPath = Path.Combine(appDirectory, "Assets/TipsToTravelChanges.db");

            // Replace TipsToTravelChanges.db with TipsToTravelDefault.db
            File.Copy(defaultDbPath, changesDbPath, true);

            // Update DatabaseFile and ConnectionString to point to TipsToTravelChanges.db
            databaseService.DatabaseFile = changesDbPath;
            databaseService.ConnectionString = $"Data Source={changesDbPath}";
        }
        private Bitmap? ConvertByteArrayToBitmap(byte[] byteArray) // Convert from Byte array to Avalonia Bitmap, so it can be displayed properly
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                return null;
            }

            using (var stream = new MemoryStream(byteArray))
            {
                return new Bitmap(stream);

            }
        }
        public override void RemovePlace(Place place)
        {
            City cityContainingPlace = countries
                .SelectMany(country => country.Cities)
                .FirstOrDefault(city => city.Places.Contains(place));

            if (cityContainingPlace != null)
            {
                cityContainingPlace.Places.Remove(place);
                Parent?.RemovePlace(place);
            }
        }
        public override void RemoveCity(City city)
        {
            Country countryContainingCity = Countries.FirstOrDefault(country => country.Cities.Contains(city));
            if (countryContainingCity != null && countryContainingCity.Cities.Remove(city))
            {
                Parent?.RemoveCity(city);
            }
        }
        public override void RemoveCountry(Country country)
        {
            if (Countries.Remove(country))
            {
                Parent?.RemoveCountry(country);
            }
        }
        public override void UpdatePlace(Place place) // Changes the place based on Id
        {

            foreach (var city in countries.SelectMany(country => country.Cities))
            {
                var placeToUpdateIndex = city.Places.FindIndex(p => p.Id == place.Id);
                if (placeToUpdateIndex != -1)
                {
                    city.Places[placeToUpdateIndex] = place;
                    Parent?.UpdatePlace(place);
                    return;
                }
            }
        }
        public override void UpdateCity(City city) // Changes the city based on Id
        {
            var countryWithCity = countries.FirstOrDefault(country => country.Cities.Any(c => c.Id == city.Id));

            if (countryWithCity != null)
            {
                int index = countryWithCity.Cities.FindIndex(c => c.Id == city.Id);
                countryWithCity.Cities[index] = city;
                Parent?.UpdateCity(city);
            }
        }
        public override void UpdateCountry(Country country) // Changes the country
        {
            int index = Countries.IndexOf(Countries.FirstOrDefault(c => c.Id == country.Id));

            if (index != -1)
            {
                Countries[index] = country;
                Parent?.UpdateCountry(country);
            }
        }
        public override void InsertPlace(Place place, int parentId) // Inserts new place
        {
            // Tries to find the city containing the place
            City cityContainingPlace = countries
                .SelectMany(country => country.Cities)
                .FirstOrDefault(city => city.Id == parentId);

            if (cityContainingPlace != null)
            {
                cityContainingPlace.Places.Add(place);
                Parent?.InsertPlace(place, parentId);
            }
        }
        public override void InsertCity(City city, int parentId) // Inserts new city
        {
            Country countryContainingCity = Countries.FirstOrDefault(country => country.Id == parentId);
            if (countryContainingCity != null)
            {
                countryContainingCity.Cities.Add(city);
                Parent?.InsertCity(city, parentId);
            }
        }
        public override void InsertCountry(Country country) // Inserts new country
        {
            country.Cities = new List<City>();
            Countries.Add(country);
            Parent?.InsertCountry(country);
        }
    }
}
