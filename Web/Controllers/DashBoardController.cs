﻿using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
