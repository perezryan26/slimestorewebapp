using SlimeStoreWeb.Contracts;
using SlimeStoreWeb.Data;

namespace SlimeStoreWeb.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
