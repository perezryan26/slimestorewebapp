using Microsoft.AspNetCore.Mvc.Rendering;

namespace SlimeStoreWeb.Models
{
    public class ItemCreateViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsFeatured { get; set; }
        public int CategoryId { get; set; }
        public SelectList? Categories { get; set; }
    }
}
