﻿using HiepStore.Models;
using HiepStore.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiepStore.Controllers.Components
{
	public class FooterViewComponent : ViewComponent
	{
		private readonly db_hiep_storeContext _context;

		public FooterViewComponent(db_hiep_storeContext context)
		{
			_context = context;
		}

		public IViewComponentResult Invoke()
		{
			HomeModel homeModel = new HomeModel();
			homeModel.ListBrand = _context.Brands
				.AsNoTracking()
				.Where(x => x.IsPublished == true && x.IsDeleted == false)
				.OrderByDescending(x => x.Ordering).ToList();
			return View(homeModel);
		}

	}
}
