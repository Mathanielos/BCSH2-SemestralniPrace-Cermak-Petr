using BCSH2SemestralniPraceCermakPetr.Models;
using BCSH2SemestralniPraceCermakPetr.Models.Enums;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2SemestralniPraceCermakPetr.ViewModels
{
    public class CityViewModel : ViewModelBase
    {
        private City showingCity;
        private List<Place> places;
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
        public List<Place> Places
        {
            get => places;
            private set => this.RaiseAndSetIfChanged(ref places, value);
        }
        public CityViewModel(City city)
        {
            ShowingCity = city;
            places = city.Places;
        }
        private void ComboBoxSelectionChanged()
        {
            if (SelectedCategory == Category.VsechnaMista)
            {
                Places = ShowingCity.Places;
            }
            else
            {
                Places = ShowingCity.Places.Where(place => place.Category == SelectedCategory).ToList();
            }
        }
    }
}
