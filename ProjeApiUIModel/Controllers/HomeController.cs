using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ProjeApiUIModel.Models;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("https://localhost:7250/");
    }

    public IActionResult AddCategory()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(CategoryViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var json = JsonConvert.SerializeObject(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/Auth/category", content);  

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Kategori eklendi.";
            return RedirectToAction("Index");
        }

        ModelState.AddModelError("", "Kategori eklenemedi.");
        return View(model);
    }
}
