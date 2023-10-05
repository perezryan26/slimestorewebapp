using System.ComponentModel.DataAnnotations.Schema;

namespace SlimeStoreWeb.Data
{
    public class Item : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsFeatured { get; set; }

        [ForeignKey("CategoryId")]
        public Category ItemCategory { get; set; }
        public int CategoryId { get; set; }
        public ICollection<Customer>? Customers { get; set; }

        public int Supply { get; set; }
    }
}
