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
    [Table("Places")]
    public class Place
    {
        [Key]
        [Column("PlaceID")]
        public int Id { get; set; }
        [Column("Name")]
        public string? Name { get; set; }
        [Column("Description")]
        public string? Description { get; set; }
        [Column("Image")]
        public Bitmap? Image { get; set; }
        [Column("CategoryID")]
        public Category? Category { get; set; }
        public Place() { }
        public Place(int id, string? name, string? description, Bitmap? image, Category? category)
        {
            Id = id;
            Name = name;
            Description = description;
            Image = image;
            Category = category;
        }
    }
}
