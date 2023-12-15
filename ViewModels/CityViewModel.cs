using BCSH2SemestralniPraceCermakPetr.Models;
using BCSH2SemestralniPraceCermakPetr.Models.Enums;
using DynamicData;
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
    public class CityViewModel : ViewModelBase
    {
        private City showingCity;
        private ObservableCollection<Place> places;
        private Category selectedCategory;
        public Category[] Categories
        {
            get
            {
                var placeCategories = ShowingCity.Places
                    .Where(place => place.Category.HasValue)
                    .Select(place => place.Category.Value)
                    .Distinct()
                    .OrderBy(category => category);

                return new[] { Category.VsechnaMista }.Concat(placeCategories).ToArray();
            }
        }
        public City ShowingCity
        {
            get => showingCity;
            private set => this.RaiseAndSetIfChanged(ref showingCity, value);
        }
        public Category SelectedCategory
        {
            get => selectedCategory;
            private set
            {
                this.RaiseAndSetIfChanged(ref selectedCategory, value);
                ComboBoxSelectionChanged();
            }
        }
        public ObservableCollection<Place> Places
        {
            get => places;
            set => this.RaiseAndSetIfChanged(ref places, value);
        }
        public CityViewModel(City city)
        {
            ShowingCity = city;
            places = new ObservableCollection<Place>(city.Places);
        }
        private void ComboBoxSelectionChanged()
        {
            if (SelectedCategory == Category.VsechnaMista)
            {
                Places = new ObservableCollection<Place>(ShowingCity.Places);
            }
            else
            {
                Places = new ObservableCollection<Place>(
                    ShowingCity.Places.Where(place => place.Category == SelectedCategory)
                );
            }
        }

        public override void RemovePlace(Place place)
        {
            if (Places.Remove(place))
            {
                this.RaisePropertyChanged(nameof(Places));
            }

            Parent?.RemovePlace(place);
        }
        public override void RemoveCity(City city)
        {
            Parent?.RemoveCity(city);
        }
    }
}
