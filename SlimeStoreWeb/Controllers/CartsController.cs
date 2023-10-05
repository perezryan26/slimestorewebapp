using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SlimeStoreWeb.Contracts;
using SlimeStoreWeb.Data;
using SlimeStoreWeb.Models;
using SlimeStoreWeb.Repositories;

namespace SlimeStoreWeb.Controllers
{
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;
        private readonly UserManager<Customer> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICartRepository cartRepository;

        public CartsController(ApplicationDbContext context, IMapper mapper, UserManager<Customer> userManager, IHttpContextAccessor httpContextAccessor, ICartRepository cartRepository)
        {
            _context = context;
            this.mapper = mapper;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.cartRepository = cartRepository;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            if(!User.Identity.IsAuthenticated)
            {
                return View();
            }

            var cart = await cartRepository.GetCart();

            var model = mapper.Map<CartViewModel>(cart);
            return View(model);
        }

        //0 is decrement, 1 is increment for operator input parameter
        public async Task<IActionResult> AdjustQuantity(int? id, int? op)
        {
            var cart = await cartRepository.GetCart();
            var item = cart.CartItems.FirstOrDefault(item => item.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            if(op == 0)
            {
                item.Quantity -= 1;
            }
            else if(op == 1)
            {
                item.Quantity += 1;
            }

            if (item.Quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                _context.Update(item);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> DeleteItem(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var cart = await cartRepository.GetCart();

            if (cart == null)
            {
                return NotFound(); // Handle cart not found
            }

            var cartItem = _context.CartItems.Find(id);

            if (cartItem == null)
            {
                return NotFound(); // Handle cart item not found
            }

            try
            {
                cart.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency conflict (if needed)
                return RedirectToAction("ConcurrencyError", new { itemId = id });
            }
            catch (Exception)
            {
                // Handle other exceptions (e.g., database errors)
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Id,DateCreated,DateUpdated")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Id,DateCreated,DateUpdated")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
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
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Carts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Carts'  is null.");
            }
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
          return (_context.Carts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
