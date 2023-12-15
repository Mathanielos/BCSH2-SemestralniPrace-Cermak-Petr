using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2SemestralniPraceCermakPetr.Models
{
    [Table("Cities")]
    public class City
    {
        [Key]
        [Column("CityId")]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BasicInformation { get; set; }
        public List<Place> Places { get; set; }
        public Bitmap Image { get; set; }

        public City(int id, string name, string description, string basicInformation, List<Place> places, Bitmap image)
        {
            Id = id;
            Name = name;
            Description = description;
            BasicInformation = basicInformation;
            Places = places;
            Image = image;

        }
    }
}
