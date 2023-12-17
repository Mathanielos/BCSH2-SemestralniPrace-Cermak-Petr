using Avalonia.Media.Imaging;
using BCSH2SemestralniPraceCermakPetr.Models;
using BCSH2SemestralniPraceCermakPetr.Models.Converters;
using BCSH2SemestralniPraceCermakPetr.Models.Enums;
using BCSH2SemestralniPraceCermakPetr.Models.Services;
using BCSH2SemestralniPraceCermakPetr.Views;
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
    public class InsertEditWindowViewModel : ViewModelBase
    {
        public event EventHandler InsertOrEditCompleted; // EventHandler to mark that insert or edit was completed and window should be closed

        private string name;
        private string description;
        private string imageFilePath;
        private string tips;
        private string basicInformation;
        private string usageName;

        private Category selectedCategory;

        private Bitmap image;
        private bool tipsVisible;
        private bool basicInformationVisible;
        private bool categoryVisible;

        private readonly DialogService dialogService;

        public ReactiveCommand<Unit, Unit> InsertEditCommand { get; }

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
        public string UsageName
        {
            get => usageName;
            set => this.RaiseAndSetIfChanged(ref usageName, value);
        }
        public Category[] Categories => ((Category[])Enum.GetValues(typeof(Category))).Skip(1).ToArray();
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
        public InsertEditWindowViewModel(object parameter, DialogService service, bool isEditing)
        {
            UpdatingObject = parameter;
            dialogService = service;
            InsertEditCommand = ReactiveCommand.Create(InsertOrUpdateData);
            TipsVisible = false;
            BasicInformationVisible = false;
            CategoryVisible = false;
            if (isEditing)
            {
                UsageName = "Editovat";
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
                    Name = country.Name;
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
            else
            {
                UsageName = "Vložit";
                if (parameter is Place place)
                {
                    CategoryVisible = true;
                }
                else if (parameter is Country country)
                {
                    TipsVisible = true;
                }
                else if (parameter is City city)
                {
                    BasicInformationVisible = true;
                }
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
        private async void InsertOrUpdateData()
        {
            try
            {
                ValidationWindow validationWindow;
                if (UpdatingObject is Place place)
                {
                    if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description))
                    {
                        await ShowValidationWindow(new ValidationViewModel("Jméno a popisek jsou povinná pole!"));
                        return;
                    }

                    place.Name = Name;
                    place.Description = Description;
                    place.Image = Image;
                    place.Category = SelectedCategory;
                    UpdatingObject = place;
                }
                else if (UpdatingObject is Country country)
                {
                    if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description) || string.IsNullOrWhiteSpace(Tips))
                    {
                        await ShowValidationWindow(new ValidationViewModel("Jméno, popisek a tipy jsou povinná pole!"));
                        return;
                    }
                    country.Name = Name;
                    country.Description = Description;
                    country.Image = Image;
                    country.Tips = Tips;
                    UpdatingObject = country;
                }
                else if (UpdatingObject is City city)
                {
                    if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description) || string.IsNullOrWhiteSpace(BasicInformation))
                    {
                        await ShowValidationWindow(new ValidationViewModel("Jméno, popisek a základní informace jsou povinná pole!"));
                        return;
                    }
                    city.Name = Name;
                    city.Description = Description;
                    city.Image = Image;
                    city.BasicInformation = BasicInformation;
                    UpdatingObject = city;
                }
                OnInsertOrEditCompleted();
            }
            catch (Exception ex)
            {
                await ShowValidationWindow(new ValidationViewModel("Nastala chyba!"));
            }
        }
        private void OnInsertOrEditCompleted()
        {
            InsertOrEditCompleted?.Invoke(this, EventArgs.Empty);
        }
        private async Task ShowValidationWindow<T>(T viewModel) where T : ViewModelBase
        {
            await dialogService.ShowValidationWindow(viewModel);
        }
    }
}
