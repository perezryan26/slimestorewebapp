using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SlimeStoreWeb.Contracts;
using SlimeStoreWeb.Data;
using SlimeStoreWeb.Data.Migrations;
using SlimeStoreWeb.Models;

namespace SlimeStoreWeb.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;
        private readonly UserManager<Customer> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IOrderRepository orderRepository;
        private readonly ICartRepository cartRepository;

        public OrdersController(ApplicationDbContext context, IMapper mapper, UserManager<Customer> userManager, IHttpContextAccessor httpContextAccessor, IOrderRepository orderRepository, ICartRepository cartRepository)
        {
            _context = context;
            this.mapper = mapper;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.orderRepository = orderRepository;
            this.cartRepository = cartRepository;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
              return _context.Orders != null ? 
                          View(await _context.Orders.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Orders'  is null.");
        }

        public async Task<IActionResult> ViewOrder(int id)
        {
            var cart = await cartRepository.GetCart();

            double cartPrice = 0;
            foreach(var item in cart.CartItems)
            {
                cartPrice += (item.Item.Price * (double)item.Quantity);;
            }

            double totalPrice = cartPrice + cartPrice * .0625;

            var userId = (await userManager.GetUserAsync(httpContextAccessor?.HttpContext?.User)).Id;

            var order = await _context.Orders
                .Include(x => x.Cart)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (order != null)
            {
                var model = mapper.Map<OrderViewModel>(order);
                model.Cart = mapper.Map<CartViewModel>(cart);
                model.CartId = id;
                model.CartPrice = cartPrice;
                model.TotalPrice = totalPrice;
                model.OrderDate = DateTime.UtcNow;
                return View(model);
            }

            var orderViewModel = new OrderCreateViewModel()
            {
                Cart = mapper.Map<CartViewModel>(cart),
                CartId = id,
                CartPrice = cartPrice,
                TotalPrice = totalPrice,
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                IsComplete = false
            };

            return View(orderViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewOrder(OrderCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var order = mapper.Map<Order>(model);

                var previousOrder = await _context.Orders
                .Include(x => x.Cart)
                .FirstOrDefaultAsync(c => c.UserId == model.UserId);

                if (previousOrder != null)
                {
                    previousOrder.CartId = model.CartId;
                    previousOrder.CartPrice = model.CartPrice;
                    previousOrder.TotalPrice = model.TotalPrice;
                    previousOrder.OrderDate = DateTime.UtcNow;
                    previousOrder.FirstName = model.FirstName;
                    previousOrder.LastName = model.LastName;
                    previousOrder.Address = model.Address;
                    previousOrder.City = model.City;
                    previousOrder.Country = model.Country;
                    previousOrder.ZipCode = model.ZipCode;
                    await orderRepository.UpdateAsync(previousOrder);
                }
                else
                {
                    _context.Add(order);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Payment));
            }

            var cart = await cartRepository.GetCart();
            model.Cart = mapper.Map<CartViewModel>(cart);

            return View(model);
        }

        public async Task<IActionResult> Payment()
        {
            var userId = (await userManager.GetUserAsync(httpContextAccessor?.HttpContext?.User)).Id;

            var order = await _context.Orders
                .Include(x => x.Cart)
                .FirstOrDefaultAsync(c => c.UserId == userId);
                
            var model = new PaymentViewModel()
            {
                CartPrice = order.CartPrice,
                TotalPrice = order.TotalPrice
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Payment(PaymentViewModel model)
        {
            if(ModelState.IsValid)
            {
                return RedirectToAction(nameof(Overview), model);
            }

            return View(model);
        }

        public async Task<IActionResult> Overview(PaymentViewModel model)
        {
            var userId = (await userManager.GetUserAsync(httpContextAccessor?.HttpContext?.User)).Id;

            var order = await _context.Orders
                .Include(x => x.Cart)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            var orderModel = mapper.Map<OrderViewModel>(order);

            return View(orderModel);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartPrice,TotalPrice,UserId,OrderDate,FirstName,LastName,Address,City,Country,ZipCode,Id,DateCreated,DateUpdated")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartPrice,TotalPrice,UserId,OrderDate,FirstName,LastName,Address,City,Country,ZipCode,Id,DateCreated,DateUpdated")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
