using SlimeStoreWeb.Data;

namespace SlimeStoreWeb.Contracts
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        public Task<Cart> GetCart();
    }
}
