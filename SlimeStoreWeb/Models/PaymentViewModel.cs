using System.ComponentModel.DataAnnotations;

namespace SlimeStoreWeb.Models
{
    public class PaymentViewModel
    {
        public int Id { get; set; }
        public double CartPrice { get; set; }
        public double TotalPrice { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string CardFirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string CardLastName { get; set; }

        [Required]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        [Required]
        [Display(Name = "Expiration Date")]
        public string ExpDate { get; set; }

        [Display(Name = "CVV")]
        [Required]
        public string Cvv { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string CardAddress { get; set; }

        [Required]
        [Display(Name = "City")]
        public string CardCity { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string CardCountry { get; set; }

        [Required]
        [Display(Name = "Zip Code")]
        public string CardZipCode { get; set; }
    }
}
