using BCSH2SemestralniPraceCermakPetr.Models.Enums;
using BCSH2SemestralniPraceCermakPetr.Models;
using BCSH2SemestralniPraceCermakPetr.Models.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.ComponentModel;
using Avalonia.Media.Imaging;

namespace BCSH2SemestralniPraceCermakPetr.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly DatabaseService databaseService;
        private ViewModelBase content;
        private Stack<ViewModelBase> viewStack; // A stack to keep track of the views
        public ReactiveCommand<CountryName, Unit> ShowCountryCommand { get; } //These 3 commands are here so views can be easily switched without issues.
        public ReactiveCommand<City, Unit> ShowCityCommand { get; }
        public ReactiveCommand<Place, Unit> ShowPlaceCommand { get; }
        public ReactiveCommand<Unit, Unit> ReturnBackCommand { get; }
        public ReactiveCommand<Unit, Unit> ReturnHomeCommand { get; }

        public MainWindowViewModel()
        {
            content = new MainViewModel();
            viewStack = new Stack<ViewModelBase>();
            ShowCountryCommand = ReactiveCommand.Create<CountryName>(ShowCountry);
            ShowCityCommand = ReactiveCommand.Create<City>(ShowCity);
            ShowPlaceCommand = ReactiveCommand.Create<Place>(ShowPlace);
            ReturnBackCommand = ReactiveCommand.Create(ReturnBack);
            ReturnHomeCommand = ReactiveCommand.Create(ReturnHome);
            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var dbPath = Path.Combine(appDirectory, "Assets/TipsToTravel.db");
            databaseService = new DatabaseService(dbPath);
            databaseService.InitializeDatabase();
        }
        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }
        private void ShowCountry(CountryName countryName)
        {
            string name = GetDescription(countryName);

            // Fetch data for the specific country from the database
            List<Dictionary<string, object>> countryData = databaseService.GetData("Countries", null, "Name = @Name", new Dictionary<string, object> { { "Name", name } });

            if (countryData.Count > 0)
            {
                var countryAttributes = countryData[0]; // Assuming there's only one row for a specific country

                // Create a Country object from the retrieved data
                Country country = new Country(
                    countryName,
                    countryAttributes["Description"] as string,
                    countryAttributes["Tips"] as string,
                    new List<City>(),
                    ConvertByteArrayToBitmap(countryAttributes["Image"] as byte[])
                );

                // Fetch city data for the specific country from the database
                List<Dictionary<string, object>> cityData = databaseService.GetData("Cities", null, "CountryID = @CountryID", new Dictionary<string, object> { { "CountryID", countryAttributes["CountryID"] } });

                foreach (var cityAttributes in cityData)
                {
                    // Create a City object for each city
                    City city = new City(
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
                            placeAttributes["Name"] as string,
                            placeAttributes["Description"] as string,
                            ConvertByteArrayToBitmap(placeAttributes["Image"] as byte[])
                        );

                        // Add the Place object to the list of places in the City
                        city.Places.Add(place);
                    }

                    // Add the City object to the list of cities in the Country
                    country.Cities.Add(city);
                }

                CountryViewModel viewModel = new CountryViewModel(country);

                viewStack.Push(content);

                Content = viewModel;
            }
        }

        private void ShowCity(City city)
        {
            CityViewModel viewModel = new CityViewModel(city);
            viewStack.Push(content);
            Content = viewModel;
        }
        private void ShowPlace(Place place)
        {
            PlaceViewModel viewModel = new PlaceViewModel(place);
            viewStack.Push(content);
            Content = viewModel;
        }
        private void ReturnBack()
        {
            if (viewStack.Count > 0)
            {
                // Pop the previous view model from the stack
                ViewModelBase previousView = viewStack.Pop();

                // Set the previous view as the current view
                Content = previousView;
            }
        }
        private void ReturnHome()
        {
            Content = new MainViewModel();
            viewStack.Clear();
        }
        private string GetDescription(CountryName countryName)
        {
            var fieldInfo = countryName.GetType().GetField(countryName.ToString());
            var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));

            if (descriptionAttribute != null)
            {
                return descriptionAttribute.Description;
            }

            return countryName.ToString(); // Fallback to enum value if no description is found
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
    }
}