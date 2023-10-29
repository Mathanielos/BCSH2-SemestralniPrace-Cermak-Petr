using BCSH2SemestralniPraceCermakPetr.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2SemestralniPraceCermakPetr.Models
{
    public class Country
    {
        public CountryName Name { get; set; }
        public string Description { get; set; }
        public string Tips { get; set; }
        public List<City> Cities { get; set; }

        public Country(CountryName name, string description, string tips, List<City> cities)
        {
            Name = name;
            Description = description;
            Tips = tips;
            Cities = cities;
        }
    }
}
