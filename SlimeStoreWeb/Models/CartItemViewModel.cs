namespace SlimeStoreWeb.Models
{
    public class CartItemViewModel
    {
        public int Id { get; set; }
        public ItemViewModel Item { get; set; }
        public int Quantity { get; set; }
    }
}
