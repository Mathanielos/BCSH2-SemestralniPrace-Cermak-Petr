using Avalonia.Media.Imaging;
using BCSH2SemestralniPraceCermakPetr.Models;
using BCSH2SemestralniPraceCermakPetr.Models.Converters;
using BCSH2SemestralniPraceCermakPetr.Models.Enums;
using BCSH2SemestralniPraceCermakPetr.Models.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reactive;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2SemestralniPraceCermakPetr.ViewModels
{
    public class EditWindowViewModel : ViewModelBase
    {
        public event EventHandler EditCompleted; // EventHandler to mark that edit was completed and window should be closed

        private string name;
        private string description;
        private string imageFilePath;
        private string tips;
        private string basicInformation;

        private Category selectedCategory;

        private Bitmap image;
        private bool tipsVisible;
        private bool basicInformationVisible;
        private bool categoryVisible;

        private readonly DialogService dialogService;

        private object updatingObject;

        public ReactiveCommand<Unit, Unit> EditCommand { get; }

        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }
        public string Description
        {
            get => description;
            set => this.RaiseAndSetIfChanged(ref description, value);
        }
        public string ImageFilePath
        {
            get => imageFilePath;
            set => this.RaiseAndSetIfChanged(ref imageFilePath, value);
        }
        public string Tips
        {
            get => tips;
            set => this.RaiseAndSetIfChanged(ref tips, value);
        }
        public string BasicInformation
        {
            get => basicInformation;
            set => this.RaiseAndSetIfChanged(ref basicInformation, value);
        }
        public Category[] Categories => (Category[])Enum.GetValues(typeof(Category));
        public Category SelectedCategory
        {
            get => selectedCategory;
            private set
            {
                this.RaiseAndSetIfChanged(ref selectedCategory, value);
            }
        }
        public Bitmap Image
        {
            get => image;
            set => this.RaiseAndSetIfChanged(ref image, value);
        }
        public object UpdatingObject { get; private set; }
        public bool TipsVisible
        {
            get => tipsVisible;
            set => this.RaiseAndSetIfChanged(ref tipsVisible, value);
        }
        public bool BasicInformationVisible
        {
            get => basicInformationVisible;
            set => this.RaiseAndSetIfChanged(ref basicInformationVisible, value);
        }
        public bool CategoryVisible
        {
            get => categoryVisible;
            set => this.RaiseAndSetIfChanged(ref categoryVisible, value);
        }
        public EditWindowViewModel(object parameter, DialogService service)
        {
            UpdatingObject = parameter;
            dialogService = service;
            EditCommand = ReactiveCommand.Create(UpdateData);
            TipsVisible = false;
            BasicInformationVisible = false;
            CategoryVisible = false;
            if (parameter is Place place)
            {
                Name = place.Name;
                Description = place.Description;
                Image = place.Image;
                SelectedCategory = (Category)place.Category;
                CategoryVisible = true;
            }
            else if (parameter is Country country)
            {
                Name = ConvertEnumToDescription(country.Name);
                Description = country.Description;
                Image = country.Image;
                Tips = country.Tips;
                TipsVisible = true;
            }
            else if (parameter is City city)
            {
                Name = city.Name;
                Description = city.Description;
                Image = city.Image;
                BasicInformation = city.BasicInformation;
                BasicInformationVisible = true;
            }
        }
        public async void ImageChoose()
        {
            var filePath = await dialogService.ShowOpenFileDialog("Výběr obrázku");

            if (!string.IsNullOrEmpty(filePath))
            {
                ImageFilePath = filePath;
                LoadImage(filePath);
            }
        }
        private void LoadImage(string filePath)
        {
            try
            {
                Image = new Bitmap(filePath); // Update the Image property
            }
            catch (Exception ex)
            {

            }
        }
        private void UpdateData()
        {
            try
            {
                if (UpdatingObject is Place place)
                {
                    place.Name = Name;
                    place.Description = Description;
                    place.Image = Image;
                    place.Category = SelectedCategory;
                    UpdatingObject = place;
                }
                else if (UpdatingObject is Country country)
                {
                    country.Name = ConvertDescriptionToEnum(Name);
                    country.Description = Description;
                    country.Image = Image;
                    country.Tips = Tips;
                    UpdatingObject = country;
                }
                else if (UpdatingObject is City city)
                {
                    city.Name = Name;
                    city.Description = Description;
                    city.Image = Image;
                    city.BasicInformation = BasicInformation;
                    UpdatingObject = city;
                }
                OnEditCompleted();
            }
            catch (Exception ex)
            {

            }
        }
        private void OnEditCompleted()
        {
            EditCompleted?.Invoke(this, EventArgs.Empty);
        }
        // Helper method to convert enum to its description
        private string ConvertEnumToDescription(object enumValue)
        {
            EnumDescriptionConverter converter = new EnumDescriptionConverter();
            return (string)converter.Convert(enumValue, typeof(string), null, CultureInfo.CurrentCulture);
        }
        // Helper method to convert enum to its description
        private CountryName ConvertDescriptionToEnum(string description)
        {
            EnumDescriptionConverter converter = new EnumDescriptionConverter();
            return (CountryName)converter.ConvertBack(description, typeof(CountryName), null, CultureInfo.CurrentCulture);
        }
    }
}
