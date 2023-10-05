using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SlimeStoreWeb.Contracts;
using SlimeStoreWeb.Data;
using SlimeStoreWeb.Models;
using System.Diagnostics;

namespace SlimeStoreWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<Customer> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICartRepository cartRepository;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IMapper mapper, UserManager<Customer> userManager, IHttpContextAccessor httpContextAccessor, ICartRepository cartRepository)
        {
            _logger = logger;
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.cartRepository = cartRepository;
        }

        public async Task<IActionResult> Index()
        {
            var featuredItems = context.Items
                .Where(item => item.IsFeatured == true)
                .ToList();

            if(featuredItems.Count > 0)
            {
                var model = mapper.Map<List<ItemViewModel>>(featuredItems);
                return View(model);
            }

            return View();
            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}