using System.ComponentModel.DataAnnotations;

namespace SlimeStoreWeb.Models
{
    public class OrderCreateViewModel
    {
        public CartViewModel? Cart { get; set; }
        public int CartId { get; set; }
        public double CartPrice { get; set; }
        public double TotalPrice { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }

        public bool IsComplete { get; set; }

        [Required]
        [Display(Name="First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }
    }
}
