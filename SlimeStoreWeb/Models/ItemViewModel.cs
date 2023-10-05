using SlimeStoreWeb.Data;

namespace SlimeStoreWeb.Models
{
    public class ItemViewModel : ItemCreateViewModel
    {
        public Category? ItemCategory { get; set; }
        public bool? IsFavorited { get; set; }
        public int Quantity { get; set; }
    }
}
