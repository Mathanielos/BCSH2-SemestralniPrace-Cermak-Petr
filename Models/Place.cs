using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2SemestralniPraceCermakPetr.Models
{
    public class Place
    {
        public string Name { get; set; }
        public int CityID { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public Bitmap Image { get; set; }
        public Place(string name, int cityID, string category, string description, Bitmap image)
        {
            Name = name;
            CityID = cityID;
            Category = category;
            Description = description;
            Image = image;
        }
    }
}
