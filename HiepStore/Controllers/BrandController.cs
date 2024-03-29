﻿using HiepStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace HiepStore.Controllers
{
    public class BrandController : Controller
    {
        private readonly ILogger<BrandController> _logger;
        private readonly db_hiep_storeContext _context;
        public BrandController(ILogger<BrandController> logger, db_hiep_storeContext context)
        {
            _logger = logger;
            _context = context;
        }
        [Route("/thuonghieu")]
        public IActionResult Index()
        {
            var ListBrands = _context.Brands.ToList();
            return View(ListBrands);
        }
    }
}
