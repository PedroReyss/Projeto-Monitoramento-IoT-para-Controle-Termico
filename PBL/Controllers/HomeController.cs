using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using PBL.Models;

namespace PBL.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Logado = HelperControllers.UserEstaLogado(HttpContext.Session);
            ViewBag.Tipo = HttpContext.Session.GetInt32("Tipo");
            return View();
        }

        public IActionResult Sobre()
        {
            ViewBag.Logado = HelperControllers.UserEstaLogado(HttpContext.Session);
            ViewBag.Tipo = HttpContext.Session.GetInt32("Tipo");
            return View();
        }
    }
}
