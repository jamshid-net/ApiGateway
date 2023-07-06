using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ApiGateway.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRabbitMqService _rabbitMqService;
        public HomeController(ILogger<HomeController> logger, IRabbitMqService rabbitMqService)
        {
            _logger = logger;
            _rabbitMqService = rabbitMqService;
        }

        public IActionResult Index(string serviceName)
        {
            ViewBag.SeriveName = serviceName;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendRabbit([FromForm] AmqpMessage mqRequest)
        {
            _rabbitMqService.SendMessage(mqRequest);
           return RedirectToAction("GetMessage", new { mqRequest.ResponseQueue });
        }
        public IActionResult GetMessage(string ResponseQueue)
        {
            string result = _rabbitMqService.GetMessage(ResponseQueue).Result?.Message??"no response!";
            ViewBag.Response=result;
           
            return View("Index");//, new { serviceName = HttpContext.Request.Query["serviceName"] });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}