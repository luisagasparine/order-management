﻿using Microsoft.AspNetCore.Mvc;

namespace OrderManagement.Controllers
{
    public class CustomersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
