using Microsoft.AspNetCore.Identity;

namespace SlimeStoreWeb.Data
{
    public class Customer : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ICollection<Item>? FavoriteItems { get; set; }
    }
}
