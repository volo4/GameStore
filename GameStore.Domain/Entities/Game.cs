using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Domain.Entities
{
    public class Game
    {
        [HiddenInput(DisplayValue = false)]
        public int GameId { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please enter the name of the game")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Please enter a description for the game")]
        public string Description { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Please select a category for the game")]
        public string Category { get; set; }

        [Display(Name = "Price (eur)")]
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive value for the price")]
        public decimal Price { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
    }
}