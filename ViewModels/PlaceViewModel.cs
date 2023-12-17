using Avalonia.Media.Imaging;
using BCSH2SemestralniPraceCermakPetr.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2SemestralniPraceCermakPetr.ViewModels
{
    public class PlaceViewModel : ViewModelBase
    {
        private ObservableCollection<Place> showingPlace;
        private Bitmap image;
        public ObservableCollection<Place> ShowingPlace
        {
            get => showingPlace;
            private set => this.RaiseAndSetIfChanged(ref showingPlace, value);
        }
        public Bitmap Image
        {
            get => image;
            private set => this.RaiseAndSetIfChanged(ref image, value);
        }
        public PlaceViewModel(Place place)
        {
            ShowingPlace = new ObservableCollection<Place>
            {
                place
            };
            Image = place.Image;
        }
        public override void RemovePlace(Place place)
        {
            Parent?.RemovePlace(place);
        }
        public override void UpdatePlace(Place place)
        {
            ShowingPlace[0] = place;
            Image = place.Image;
            Parent?.UpdatePlace(place);
        }
    }
}
