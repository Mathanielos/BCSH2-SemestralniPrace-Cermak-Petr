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
using BCSH2SemestralniPraceCermakPetr.Views;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BCSH2SemestralniPraceCermakPetr.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly DatabaseService databaseService;
        private readonly DialogService dialogService;
        private ViewModelBase content;
        private Stack<ViewModelBase> viewStack; // A stack to keep track of the views
        public ReactiveCommand<string, Unit> ShowCountryCommand { get; } //This and next 2 commands are here so views can be easily switched without issues.
        public ReactiveCommand<City, Unit> ShowCityCommand { get; }
        public ReactiveCommand<Place, Unit> ShowPlaceCommand { get; }
        public ReactiveCommand<Unit, Unit> ReturnBackCommand { get; }
        public ReactiveCommand<Unit, Unit> ReturnHomeCommand { get; }
        public ReactiveCommand<Unit, Unit> InsertCommand { get; } // Insert button in the menu at the top
        public ReactiveCommand<Unit, Unit> EditCommand { get; } // Edit button in the menu at the top
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; } // Delete button in the menu at the top
        public ReactiveCommand<Unit, Unit> SelectCommand { get; } // Select button in the menu at the top
        public ReactiveCommand<object, Unit> EditDataCommand { get; } // Editation of the object
        public ReactiveCommand<object, Unit> DeleteDataCommand { get; } // Deletion of the object

        // Visibility of editing and deleting buttons
        private bool isEditButtonVisible;
        private bool isDeleteButtonVisible;

        // Changing between editing/deleting or selecting
        private bool isNotEditing;
        private bool isNotDeleting;
        private bool isNotSelecting;

        public bool IsEditButtonVisible
        {
            get => isEditButtonVisible;
            set => this.RaiseAndSetIfChanged(ref isEditButtonVisible, value);
        }
        public bool IsDeleteButtonVisible
        {
            get => isDeleteButtonVisible;
            set => this.RaiseAndSetIfChanged(ref isDeleteButtonVisible, value);
        }
        public bool IsNotEditing
        {
            get => isNotEditing;
            set => this.RaiseAndSetIfChanged(ref isNotEditing, value);
        }
        public bool IsNotDeleting
        {
            get => isNotDeleting;
            set => this.RaiseAndSetIfChanged(ref isNotDeleting, value);
        }
        public bool IsNotSelecting
        {
            get => isNotSelecting;
            set => this.RaiseAndSetIfChanged(ref isNotSelecting, value);
        }

        public MainWindowViewModel()
        {
            content = new MainViewModel();
            viewStack = new Stack<ViewModelBase>();
            ShowCountryCommand = ReactiveCommand.Create<string>(ShowCountry);
            ShowCityCommand = ReactiveCommand.Create<City>(ShowCity);
            ShowPlaceCommand = ReactiveCommand.Create<Place>(ShowPlace);
            ReturnBackCommand = ReactiveCommand.Create(ReturnBack);
            ReturnHomeCommand = ReactiveCommand.Create(ReturnHome);
            InsertCommand = ReactiveCommand.Create(Insert);
            EditCommand = ReactiveCommand.Create(Edit);
            DeleteCommand = ReactiveCommand.Create(Delete);
            SelectCommand = ReactiveCommand.Create(Select);
            EditDataCommand = ReactiveCommand.Create<object>(EditData);
            DeleteDataCommand = ReactiveCommand.Create<object>(DeleteData);
            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var dbPath = Path.Combine(appDirectory, "Assets/TipsToTravelChanges.db");
            databaseService = new DatabaseService(dbPath);
            databaseService.InitializeDatabase();
            dialogService = new DialogService();
            IsEditButtonVisible = false;
            IsDeleteButtonVisible = false;
            IsNotEditing = true;
            IsNotDeleting = true;
            IsNotSelecting = false;
        }
        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }
        private void ShowCountry(string countryName)
        {

            // Fetch data for the specific country from the database
            List<Dictionary<string, object>> countryData = databaseService.GetData("Countries", null, "Name = @Name", new Dictionary<string, object> { { "Name", countryName } });

            if (countryData.Count > 0)
            {
                var countryAttributes = countryData[0]; // Assuming there's only one row for a specific country

                // Create a Country object from the retrieved data
                Country country = new(
                    Convert.ToInt32(countryAttributes["CountryID"]),
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

                CountryViewModel viewModel = new CountryViewModel(country);
                viewModel.SetParent(content);
                viewStack.Push(content);

                Content = viewModel;
            }
        }

        private void ShowCity(City city)
        {
            CityViewModel viewModel = new CityViewModel(city);
            viewModel.SetParent(content);
            viewStack.Push(content);
            Content = viewModel;
        }
        private void ShowPlace(Place place)
        {
            PlaceViewModel viewModel = new PlaceViewModel(place);
            viewModel.SetParent(content);
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
        private async void Insert()
        {
            InsertEditWindowViewModel insertEditWindow;

            // If specific country is showing then insert will be new city to the country
            if (Content is CountryViewModel countryViewModel)
            {
                int insertingToCountryId = countryViewModel.ShowingCountry[0].Id;
                City newCity = new City();
                insertEditWindow = new InsertEditWindowViewModel(newCity, dialogService, false);
                await ShowInsertEditWindow(insertEditWindow);
                newCity = (City)insertEditWindow.UpdatingObject;
                newCity.Id = databaseService.InsertData(newCity, insertingToCountryId);
                Content?.InsertCity(newCity, insertingToCountryId);
            }

            // If specific city is showing then insert will be new place to the city
            else if (Content is CityViewModel cityViewModel)
            {
                int insertingToCityId = cityViewModel.ShowingCity[0].Id;
                Place newPlace = new Place();
                insertEditWindow = new InsertEditWindowViewModel(newPlace, dialogService, false);
                await ShowInsertEditWindow(insertEditWindow);
                newPlace = (Place)insertEditWindow.UpdatingObject;
                newPlace.Id = databaseService.InsertData(newPlace, insertingToCityId);
                Content?.InsertPlace(newPlace, insertingToCityId);
            }

        }
        private void Edit()
        {
            IsEditButtonVisible = true;
            IsDeleteButtonVisible = false;

            IsNotSelecting = true;
            IsNotEditing = false;
            IsNotDeleting = true;
        }
        private void Delete()
        {
            IsEditButtonVisible = false;
            IsDeleteButtonVisible = true;

            IsNotSelecting = true;
            IsNotEditing = true;
            IsNotDeleting = false;
        }
        private void Select()
        {
            IsEditButtonVisible = false;
            IsDeleteButtonVisible = false;

            IsNotSelecting = false;
            IsNotEditing = true;
            IsNotDeleting = true;
        }
        private async void EditData(object parameter)
        {
            InsertEditWindowViewModel insertEditWindow;
            if (parameter is Place place)
            {

                insertEditWindow = new InsertEditWindowViewModel(place, dialogService, true);
                await ShowInsertEditWindow(insertEditWindow);
                Place updatedPlace = (Place)insertEditWindow.UpdatingObject;
                databaseService.UpdateData(updatedPlace);
                Content?.UpdatePlace(updatedPlace);
            }
            else if (parameter is Country country)
            {
                insertEditWindow = new InsertEditWindowViewModel(country, dialogService, true);
                await ShowInsertEditWindow(insertEditWindow);
                Country updatedCountry = (Country)insertEditWindow.UpdatingObject;
                databaseService.UpdateData(updatedCountry);
                Content?.UpdateCountry(updatedCountry);
            }
            else if (parameter is City city)
            {
                insertEditWindow = new InsertEditWindowViewModel(city, dialogService, true);
                await ShowInsertEditWindow(insertEditWindow);
                City updatedCity = (City)insertEditWindow.UpdatingObject;
                databaseService.UpdateData(updatedCity);
                Content?.UpdateCity(updatedCity);
            }
        }
        private void DeleteData(object parameter)
        {
            if (parameter is Place place)
            {
                databaseService.DeleteData(place);

                Content?.RemovePlace(place);
                if (Content is PlaceViewModel placeViewModel)
                {
                    ReturnBack();
                }
            }
            else if (parameter is Country country)
            {
                databaseService.DeleteData(country);
                ReturnBack();
            }
            else if (parameter is City city)
            {
                databaseService.DeleteData(city);

                Content?.RemoveCity(city);
                if (Content is CityViewModel cityViewModel)
                {
                    ReturnBack();
                }
            }
        }
        private async Task ShowInsertEditWindow<T>(T viewModel) where T : ViewModelBase
        {
            await dialogService.ShowInsertEditWindow(viewModel);
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