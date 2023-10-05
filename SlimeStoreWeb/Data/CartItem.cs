namespace SlimeStoreWeb.Data
{
    public class CartItem : BaseEntity
    {
        public Item Item { get; set; }
        public int Quantity { get; set; }
    }
}
