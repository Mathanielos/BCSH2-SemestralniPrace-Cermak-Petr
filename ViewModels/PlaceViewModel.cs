using BCSH2SemestralniPraceCermakPetr.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2SemestralniPraceCermakPetr.ViewModels
{
    public class PlaceViewModel : ViewModelBase
    {
        private Place showingPlace;
        public Place ShowingPlace
        {
            get => showingPlace;
            private set => this.RaiseAndSetIfChanged(ref showingPlace, value);
        }
        public PlaceViewModel(Place place)
        {
            ShowingPlace = place;
        }
        public override void RemovePlace(Place place)
        {
            Parent?.RemovePlace(place);
        }
        public override void UpdatePlace(Place place)
        {
            ShowingPlace = place;
            this.RaisePropertyChanged(nameof(ShowingPlace));
            Parent?.UpdatePlace(place);
        }
    }
}
