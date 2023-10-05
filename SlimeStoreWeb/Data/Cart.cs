namespace SlimeStoreWeb.Data
{
    public class Cart : BaseEntity
    {
        public List<CartItem> CartItems { get; set; }
        public string UserId { get; set; }
    }
}
