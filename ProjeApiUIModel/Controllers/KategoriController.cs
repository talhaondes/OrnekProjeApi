using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjeApiModel.DTOs.Data;
using ProjeApiModel.DTOs.Response;

namespace ProjeApiUIModel.Controllers
{
    public class KategoriController : Controller
    {
        private readonly HttpClient _httpClient;

        public KategoriController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            // API projesinin çalıştığı adres; API portunuzla uyumlu olduğundan emin olun.
            _httpClient.BaseAddress = new Uri("https://localhost:7250/");
        }

        // 1) Kategori listesini görüntüleme
        public async Task<IActionResult> Index()
        {
            // Doğru endpoint: "api/Auth/CategoryGet"
            var response = await _httpClient.GetAsync("api/Auth/CategoryGet");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var responseDto = JsonConvert.DeserializeObject<ResponseDTO<List<CategoryDTO>>>(json);
                var categories = responseDto?.Data ?? new List<CategoryDTO>();

                return View(categories);
            }

            // Hata olursa boş liste dön
            return View(new List<CategoryDTO>());
        }

        // 2) Kategori ekleme formu (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 2) Kategori ekleme işlemi (POST)
        [HttpPost]
        public async Task<IActionResult> Create(CategoryDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            // API'deki kategori ekleme endpoint'ine POST isteği gönderiyoruz.
            var response = await _httpClient.PostAsync("api/Auth/CategoryAdd", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                // Hata mesajını loglayın veya hata mesajını ModelState'e ekleyin
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Kategori eklenirken hata oluştu: {errorContent}");
            }
            return View(model);
        }
        // Kategori düzenleme formu
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/Auth/GetByIdCategory/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var responseDto = JsonConvert.DeserializeObject<ResponseDTO<CategoryDTO>>(json);

                return View(responseDto?.Data);
            }

            return NotFound();
        }

        // Kategori düzenleme işlemi
        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/Auth/CategoryEdit/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Kategori güncellenirken hata oluştu: {errorContent}");
            return View(model);
        }


        // Kategori silme işlemi
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Auth/CategoryDelete/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Kategori silinirken hata oluştu: {errorContent}");
            }

            return RedirectToAction("Index");
        }

    }
}
