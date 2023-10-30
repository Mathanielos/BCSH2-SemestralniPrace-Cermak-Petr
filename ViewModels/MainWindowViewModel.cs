using BCSH2SemestralniPraceCermakPetr.Models.Enums;
using BCSH2SemestralniPraceCermakPetr.Models;
using BCSH2SemestralniPraceCermakPetr.Models.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.ComponentModel;

namespace BCSH2SemestralniPraceCermakPetr.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly DatabaseService databaseService;
        private ViewModelBase content;
        public ReactiveCommand<CountryName, Unit> ShowCountryCommand { get; } //These 3 commands are here so views can be easily switched without issues.
        public ReactiveCommand<Unit, Unit> ShowCityCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowPlaceCommand { get; }

        public MainWindowViewModel()
        {
            content = new MainViewModel();
            ShowCountryCommand = ReactiveCommand.Create<CountryName>(ShowCountry);
            ShowCityCommand = ReactiveCommand.Create(ShowCity);
            ShowPlaceCommand = ReactiveCommand.Create(ShowPlace);
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
            CountryViewModel viewModel = new CountryViewModel(name);
            Content = viewModel;
        }
        private void ShowCity()
        {
            CityViewModel viewModel = new CityViewModel();
            Content = viewModel;
        }
        private void ShowPlace()
        {
            PlaceViewModel viewModel = new PlaceViewModel();
            Content = viewModel;
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
    }
}