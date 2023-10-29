using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2SemestralniPraceCermakPetr.Models
{
    public class City
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string BasicInformation { get; set; }
        public Bitmap Image { get; set; }

        public City(string name, string description, string basicInformation, Bitmap image)
        {
            Name = name;
            Description = description;
            BasicInformation = basicInformation;
            Image = image;
        }
    }
}
