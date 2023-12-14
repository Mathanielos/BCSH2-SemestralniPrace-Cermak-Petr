using Avalonia.Media.Imaging;
using BCSH2SemestralniPraceCermakPetr.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2SemestralniPraceCermakPetr.Models
{
    public class Place
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Bitmap? Image { get; set; }
        public Category? Category { get; set; }
        public Place(string? name, string? description, Bitmap? image, Category? category)
        {
            Name = name;
            Description = description;
            Image = image;
            Category = category;
        }
    }
}
