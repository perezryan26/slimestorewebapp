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

namespace SlimeStoreWeb.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;
        private readonly UserManager<Customer> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICartRepository cartRepository;

        public ItemsController(ApplicationDbContext context, IMapper mapper, UserManager<Customer> userManager, IHttpContextAccessor httpContextAccessor, ICartRepository cartRepository)
        {
            _context = context;
            this.mapper = mapper;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.cartRepository = cartRepository;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            var Items = await _context.Items
                .Include(q => q.ItemCategory)
                .ToListAsync();

            var model = mapper.Map<List<ItemViewModel>>(Items);
            return View(model);
        }

        public async Task<IActionResult> ItemsView()
        {
            var Items = await _context.Items
                .Include(q => q.ItemCategory)
                .ToListAsync();

            var model = mapper.Map<List<ItemViewModel>>(Items);
            return View(model);
        }

        public async Task<IActionResult> AddToCart(int? id, int quantity)
        {
            if(id == null)
            {
                return NotFound();
            }

            if(!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Details), new { id = id });
            }

            var cart = await cartRepository.GetCart();
            var existingItem = cart.CartItems.FirstOrDefault(item => item.Item.Id == id);
            //var existingItem = await _context.CartItems.FirstOrDefaultAsync(item => item.Item.Id == id);
            var item = await _context.Items.FindAsync(id);

            if(item == null)
            {
                return NotFound();
            }

            var cartItem = new CartItem()
            {
                Quantity = quantity,
                Item = item
            };

            if (cart != null) //cart exists
            {
                if(existingItem != null) //item exists in cart
                {
                    existingItem.Quantity += quantity;

                    _context.Update(existingItem);
                }
                else //item doesn't exist in cart
                {
                    cart.CartItems.Add(cartItem);

                    _context.Update(cart);
                }
            }
            else //cart doesn't exist
            {
                var userId = (await userManager.GetUserAsync(httpContextAccessor?.HttpContext?.User)).Id;

                var newCart = new Cart()
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                    {
                        cartItem
                    }
                };

                await cartRepository.AddAsync(newCart);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = id });
        }

        public async Task<IActionResult> AddToCartQuick(int? id, string returnUrl)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var item = await _context.Items.FindAsync(id);

                if (item == null)
                {
                    return NotFound();
                }

                if (!User.Identity.IsAuthenticated)
                {
                    return Redirect(returnUrl);
                }

                var cart = await cartRepository.GetCart();

                var existingItem = cart.CartItems.FirstOrDefault(item => item.Item.Id == id);

                if (existingItem == null)
                {
                    var cartItem = new CartItem()
                    {
                        Item = item,
                        Quantity = 1
                    };

                    if (cart == null)
                    {
                        var userId = (await userManager.GetUserAsync(httpContextAccessor?.HttpContext?.User)).Id;

                        cart = new Cart()
                        {
                            UserId = userId,
                            CartItems = new List<CartItem>() { cartItem }
                        };

                        await cartRepository.AddAsync(cart);
                    }
                    else
                    {
                        cart.CartItems.Add(cartItem);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    existingItem.Quantity += 1;
                    await _context.SaveChangesAsync();
                }

                return Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, and return an appropriate error response.
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred.");
            }
        }


        public async Task<IActionResult> ItemsFavoriteView()
        {
            if(User.Identity.IsAuthenticated)
            {
                var userId = (await userManager.GetUserAsync(httpContextAccessor?.HttpContext?.User)).Id;
                var user = await _context.Users
                    .Include(p => p.FavoriteItems)
                    .FirstOrDefaultAsync(q => q.Id == userId);

                if (user == null)
                {
                    return NotFound();
                }

                var model = mapper.Map<List<ItemViewModel>>(user.FavoriteItems);
                return View(model);
            }
            return View();
            
        }


        public async Task<IActionResult> FavoriteItem(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = _context.Items.FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            if(User.Identity.IsAuthenticated)
            {
                var userId = (await userManager.GetUserAsync(httpContextAccessor?.HttpContext?.User)).Id;
                var user = await _context.Users
                    .Include(p => p.FavoriteItems)
                    .FirstOrDefaultAsync(q => q.Id == userId);

                if (user == null)
                {
                    return NotFound();
                }

                if (user.FavoriteItems.Contains(item))
                {
                    user.FavoriteItems.Remove(item);
                }
                else
                {
                    user.FavoriteItems.Add(item);
                }

                _context.Update(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var relatedItems = await _context.Items
                                .Where(q => q.IsFeatured == true)
                                .ToListAsync();
            
            if(relatedItems == null)
            {
                ViewBag.RelatedItems = null;
            }
            else
            {
                var relatedItemsViewModel = mapper.Map<List<ItemViewModel>>(relatedItems);
                ViewBag.RelatedItems = relatedItemsViewModel;
            }

            var model = mapper.Map<ItemViewModel>(item);

            if (User.Identity.IsAuthenticated)
            {
                var userId = (await userManager.GetUserAsync(httpContextAccessor?.HttpContext?.User)).Id;
                var user = await _context.Users
                    .Include(p => p.FavoriteItems)
                    .FirstOrDefaultAsync(q => q.Id == userId);

                if (user == null)
                {
                    return NotFound();
                }

                if (user.FavoriteItems.Contains(item))
                {
                    model.IsFavorited = true;
                }
                else
                {
                    model.IsFavorited = false;
                }
            }
            return View(model);
            
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();
            var model = new ItemCreateViewModel()
            {
                Categories = new SelectList(categories, "Id", "Name")
            };

            return View(model);
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var item = mapper.Map<Item>(model);
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var categories = _context.Categories.ToList();
            model.Categories = new SelectList(categories, "Id", "Name");
            return View(model);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            var model = mapper.Map<ItemViewModel>(item);

            var categories = _context.Categories.ToList();
            model.Categories = new SelectList(categories, "Id", "Name");

            return View(model);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ItemViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(model.CategoryId);
            model.ItemCategory = category;

            if (ModelState.IsValid)
            {
                try
                {
                    var item = mapper.Map<Item>(model);
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(model.Id))
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
            var categories = _context.Categories.ToList();
            model.Categories = new SelectList(categories, "Id", "Name");
            return View(model);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Items == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Items'  is null.");
            }
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return (_context.Items?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
