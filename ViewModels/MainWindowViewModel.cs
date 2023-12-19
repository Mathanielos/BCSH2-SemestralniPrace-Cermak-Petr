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
using System.Xml.Linq;
using System.Linq;
using System.Collections;

namespace BCSH2SemestralniPraceCermakPetr.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly DatabaseService databaseService;
        private readonly DialogService dialogService;
        private ViewModelBase content;
        private Stack<ViewModelBase> viewStack; // A stack to keep track of the views
        public ReactiveCommand<Country, Unit> ShowCountryCommand { get; } // This and next 2 commands are here so views can be easily switched without issues
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
        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }
        public MainWindowViewModel()
        {
            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var dbPath = Path.Combine(appDirectory, "Assets/TipsToTravelChanges.db");
            databaseService = new DatabaseService(dbPath);
            databaseService.InitializeDatabase();
            content = new MainViewModel(databaseService);
            viewStack = new Stack<ViewModelBase>();
            ShowCountryCommand = ReactiveCommand.Create<Country>(ShowCountry);
            ShowCityCommand = ReactiveCommand.Create<City>(ShowCity);
            ShowPlaceCommand = ReactiveCommand.Create<Place>(ShowPlace);
            ReturnBackCommand = ReactiveCommand.Create(ReturnBack);
            ReturnHomeCommand = ReactiveCommand.Create(ReturnHome);
            InsertCommand = ReactiveCommand.CreateFromTask(Insert);
            EditCommand = ReactiveCommand.Create(Edit);
            DeleteCommand = ReactiveCommand.Create(Delete);
            SelectCommand = ReactiveCommand.Create(Select);
            EditDataCommand = ReactiveCommand.CreateFromTask<object>(EditData);
            DeleteDataCommand = ReactiveCommand.Create<object>(DeleteData);
            dialogService = new DialogService();
            IsEditButtonVisible = false;
            IsDeleteButtonVisible = false;
            IsNotEditing = true;
            IsNotDeleting = true;
            IsNotSelecting = false;
        }
        private void ShowCountry(Country country)
        {
            CountryViewModel viewModel = new CountryViewModel(country);
            viewModel.SetParent(content);
            viewStack.Push(content);

            Content = viewModel;
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
            if (viewStack.Count > 0)
            {

                // Create a new stack with the old stack so it will be reversed
                viewStack = new Stack<ViewModelBase>(viewStack);

                // Get the bottommost element
                ViewModelBase bottommostView = viewStack.Pop();

                // Set the bottommost view as the current view
                Content = bottommostView;
            }
            else
            {
                // If the viewStack is empty, set the Content to a new MainViewModel
                Content = new MainViewModel(databaseService);
            }

            // Clear the viewStack
            viewStack.Clear();
        }
        private async Task Insert()
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
                if (string.IsNullOrWhiteSpace(newCity.Name))
                {
                    return;
                }
                else
                {
                    newCity.Id = databaseService.InsertData(newCity, insertingToCountryId);
                    Content?.InsertCity(newCity, insertingToCountryId);
                }
            }
            // If in menu then insert will be new country
            else if (Content is MainViewModel mainViewModel)
            {
                Country newCountry = new Country();
                insertEditWindow = new InsertEditWindowViewModel(newCountry, dialogService, false);
                await ShowInsertEditWindow(insertEditWindow);
                newCountry = (Country)insertEditWindow.UpdatingObject;
                if (string.IsNullOrWhiteSpace(newCountry.Name))
                {
                    return;
                }
                else
                {
                    newCountry.Id = databaseService.InsertData(newCountry);
                    Content?.InsertCountry(newCountry);
                }
            }
            // If specific city is showing then insert will be new place to the city
            else if (Content is CityViewModel cityViewModel)
            {
                int insertingToCityId = cityViewModel.ShowingCity[0].Id;
                Place newPlace = new Place();
                insertEditWindow = new InsertEditWindowViewModel(newPlace, dialogService, false);
                await ShowInsertEditWindow(insertEditWindow);
                newPlace = (Place)insertEditWindow.UpdatingObject;
                if (string.IsNullOrWhiteSpace(newPlace.Name))
                {
                    return;
                }
                else
                {
                    newPlace.Id = databaseService.InsertData(newPlace, insertingToCityId);
                    Content?.InsertPlace(newPlace, insertingToCityId);
                }
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
        private async Task EditData(object parameter)
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
                if (Content is PlaceViewModel)
                {
                    ReturnBack();
                }
            }
            else if (parameter is Country country)
            {
                databaseService.DeleteData(country);
                Content?.RemoveCountry(country);
                if (Content is CountryViewModel)
                {
                    ReturnBack();
                }
            }
            else if (parameter is City city)
            {
                databaseService.DeleteData(city);

                Content?.RemoveCity(city);
                if (Content is CityViewModel)
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