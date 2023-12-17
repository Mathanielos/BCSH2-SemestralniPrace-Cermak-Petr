using Avalonia.Media.Imaging;
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
        private ObservableCollection<City> showingCity;
        private Bitmap image;
        private ObservableCollection<Place> places;
        private Category selectedCategory;
        public Category[] Categories
        {
            get
            {
                var placeCategories = ShowingCity[0].Places
                    .Where(place => place.Category.HasValue)
                    .Select(place => place.Category.Value)
                    .Distinct()
                    .OrderBy(category => category);

                return new[] { Category.VsechnaMista }.Concat(placeCategories).ToArray();
            }
        }
        public ObservableCollection<City> ShowingCity
        {
            get => showingCity;
            private set => this.RaiseAndSetIfChanged(ref showingCity, value);
        }
        public Bitmap Image
        {
            get => image;
            private set => this.RaiseAndSetIfChanged(ref image, value);
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
            ShowingCity = new ObservableCollection<City>
            {
                city
            };
            Image = city.Image;
            places = new ObservableCollection<Place>(city.Places);
        }
        private void ComboBoxSelectionChanged()
        {
            if (SelectedCategory == Category.VsechnaMista)
            {
                Places = new ObservableCollection<Place>(ShowingCity[0].Places);
            }
            else
            {
                Places = new ObservableCollection<Place>(
                    ShowingCity[0].Places.Where(place => place.Category == SelectedCategory)
                );
            }
        }

        public override void RemovePlace(Place place)
        {
            if (Places.Remove(place))
            {
                Parent?.RemovePlace(place);
            }
        }
        public override void RemoveCity(City city)
        {
            Parent?.RemoveCity(city);
        }
        public override void UpdatePlace(Place place) // Changes the place, it needs to check if the place was found or not based on Id
        {
            int index = Places.IndexOf(Places.FirstOrDefault(p => p.Id == place.Id));

            if (index != -1)
            {
                Places[index] = place;
                Parent?.UpdatePlace(place);
            }
        }
        public override void UpdateCity(City city) // Changes the city
        {
            ShowingCity[0] = city;
            Image = city.Image;
            Parent?.UpdateCity(city);
        }
        public override void InsertPlace(Place place, int parentId) // Inserts new place
        {
            Places.Add(place);
            Parent?.InsertPlace(place, parentId);
        }
    }
}
