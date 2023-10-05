using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SlimeStoreWeb.Contracts;
using SlimeStoreWeb.Data;

namespace SlimeStoreWeb.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<Customer> userManager;

        public CartRepository(ApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, UserManager<Customer> userManager) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public async Task<Cart> GetCart()
        {
            var userId = (await userManager.GetUserAsync(httpContextAccessor?.HttpContext?.User)).Id;

            var cart = await context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ic => ic.Item)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return cart;
        }
    }
}
