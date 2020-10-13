using System.ComponentModel.DataAnnotations;

namespace GameStore.Domain.Entities
{
    public class ShippingDetails
    {
        [Required(ErrorMessage = "Your name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Insert the first shipping address")]
        [Display(Name = "1 Adress")]
        public string Line1 { get; set; }

        [Display(Name = "2 Adress")]
        public string Line2 { get; set; }

        [Display(Name = "3 Adress")]
        public string Line3 { get; set; }

        [Required(ErrorMessage = "Your city")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Your country")]
        [Display(Name = "Country")]
        public string Country { get; set; }

        public bool GiftWrap { get; set; }
    }
}