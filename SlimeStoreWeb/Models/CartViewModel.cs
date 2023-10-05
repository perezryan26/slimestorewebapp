namespace SlimeStoreWeb.Models
{
    public class CartViewModel
    {
        public int Id { get; set; }
        public List<CartItemViewModel> CartItems { get; set; }
        public string UserId { get; set; }
    }
}
