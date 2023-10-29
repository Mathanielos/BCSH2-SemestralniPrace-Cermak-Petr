using BCSH2SemestralniPraceCermakPetr.Models.Enums;
using BCSH2SemestralniPraceCermakPetr.Models;
using BCSH2SemestralniPraceCermakPetr.Models.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;

namespace BCSH2SemestralniPraceCermakPetr.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private DatabaseService databaseService;
        private ViewModelBase content;
        public ReactiveCommand<Unit, Unit> ShowCountryCommand { get; } //Tyto 3 Příkazy slouží ke změnám specifických oken, lze to udělat i bez nich s použitím veřejných metod.
        public ReactiveCommand<Unit, Unit> ShowCityCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowPlaceCommand { get; }

        public MainWindowViewModel()
        {
            content = new MainViewModel();
            ShowCountryCommand = ReactiveCommand.Create(ShowCountry);
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
        private void ShowCountry()
        {
            CountryViewModel viewModel = new CountryViewModel();
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
    }
}