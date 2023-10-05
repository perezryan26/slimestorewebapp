using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.ComponentModel.DataAnnotations.Schema;

namespace SlimeStoreWeb.Data
{
    public class Order : BaseEntity
    {
        [ForeignKey("CartId")]
        public Cart Cart { get; set; }
        public int CartId { get; set; }
        public double CartPrice { get; set; }
        public double TotalPrice { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsComplete { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }
}
