using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MonitoradorLogs.Models;
using System.Collections.Generic;
using System.Diagnostics;
using MonitoradorLogs.Models;


namespace MonitoradorLogs.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LogUpdater _logUpdater;

        public HomeController(ILogger<HomeController> logger, LogUpdater logUpdater)
        {
            _logger = logger;
            _logUpdater = logUpdater;
        }

        public async Task<IActionResult> Index()
        {
            await EsperaCarregarDados();
            ViewBag.ult_atualizacao = _logUpdater.ult_atualizacao;
            return View(_logUpdater.Logs);
        }

        private async Task EsperaCarregarDados()
        {
            int tentativa = 1;
            int delayTempo = 3000;

            while (_logUpdater.Logs.Count <= 0 && tentativa <= 3)
            {
                await Task.Delay(delayTempo);
                tentativa++;
                delayTempo *= 2;
            }
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
