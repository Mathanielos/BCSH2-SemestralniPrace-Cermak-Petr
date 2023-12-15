using Avalonia.Media.Imaging;
using BCSH2SemestralniPraceCermakPetr.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2SemestralniPraceCermakPetr.Models
{
    [Table("Countries")]
    public class Country
    {
        [Key]
        [Column("CountryId")]
        public int Id { get; set; }
        public CountryName Name { get; set; }
        public string Description { get; set; }
        public string Tips { get; set; }
        public List<City> Cities { get; set; }
        public Bitmap Image { get; set; }

        public Country(int id, CountryName name, string description, string tips, List<City> cities, Bitmap image)
        {
            Id = id;
            Name = name;
            Description = description;
            Tips = tips;
            Cities = cities;
            Image = image;
        }
    }
}
