using AgricolaWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Net;


namespace AgricolaWeb.Controllers
{
    public class LotesController : Controller
    {
        private readonly HttpClient _httpClient;

        public LotesController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> GetByFarmId(int farmId)
        {
            
            var response = await _httpClient.GetAsync($"https://localhost:7274/Lotes/GetByFarmId?farmId={farmId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var lotes = JsonSerializer.Deserialize<List<Lotes>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                TempData["FarmId"] = farmId;
                return View(lotes);
            }

            return View(new List<Lotes>());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"https://localhost:7274/Lotes/Details?id={id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var lote = JsonSerializer.Deserialize<Lotes>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(lote);
            }

            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var response = await _httpClient.GetAsync($"https://localhost:7274/Lotes/Details?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var lote = JsonSerializer.Deserialize<Lotes>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (lote != null)
                {
                    return View(lote); 
                }
            }

            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, Lotes lote)
        {
            if (!ModelState.IsValid)
            {
                return View(lote);
            }

            var jsonContent = new StringContent(JsonSerializer.Serialize(lote), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"https://localhost:7274/Lotes/Edit/?id={id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetByFarmId", new { farmId = lote.IdFinca });
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"API error: {errorResponse}");

            return View(lote);
        }



        [HttpGet]
        public IActionResult Create()
        {
            
            if (TempData["FarmId"] != null)
            {
                ViewData["FarmId"] = TempData["FarmId"];
                TempData.Keep("FarmId");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Lotes lote)
        {
            if (!ModelState.IsValid)
            {
                if (TempData["FarmId"] != null)
                {
                    ViewData["FarmId"] = TempData["FarmId"];
                    TempData.Keep("FarmId"); 
                }

                return View(lote);
            }

           
            int farmId = (int)TempData["FarmId"];
            lote.IdFinca = farmId;

            var jsonContent = new StringContent(JsonSerializer.Serialize(lote), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:7274/Lotes/Create", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Lotes>(jsonString);
                return RedirectToAction("GetByFarmId", new { farmId = lote.IdFinca });
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"API error: {errorResponse}");
            if (TempData["FarmId"] != null)
            {
                ViewData["FarmId"] = TempData["FarmId"];
                TempData.Keep("FarmId"); // Keep TempData value for the subsequent request
            }


            return View(lote);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lote = await _httpClient.GetFromJsonAsync<Lotes>($"https://localhost:7274/Lotes/Details?id={id}");

            if (lote == null)
            {
                return NotFound();
            }

            return View(lote);
        }



        [HttpPost, ActionName("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7274/Lotes/Delete?id={id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetByFarmId");
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"API error: {errorResponse}");
            return RedirectToAction("GetByFarmId");
        }









    }
}
