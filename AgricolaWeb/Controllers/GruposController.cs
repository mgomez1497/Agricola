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
    public class GruposController : Controller
    {
        private readonly HttpClient _httpClient;

        public GruposController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public ActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetByLotId(int id)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7274/Grupos/GetByLotId?lotId={id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                var grupos = JsonSerializer.Deserialize<List<Grupos>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                TempData["LotId"] = id;
                TempData.Keep("LotId");

                return View(grupos);
            }

            return View(new List<Grupos>());
        }
    

    // GET: GruposController/Details/5
    public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"https://localhost:7274/Grupos/Details?id={id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var grupo = JsonSerializer.Deserialize<Grupos>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(grupo);
            }

            return NotFound();
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (TempData["LotId"] != null)
            {
                ViewData["LotId"] = TempData["LotId"];
                TempData.Keep("LotId");
            }
            else
            {
               
                ModelState.AddModelError(string.Empty, "LotId no encontrado.");
                return RedirectToAction("Index", "Lotes");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Grupos grupo)
        {
            if (!ModelState.IsValid)
            {
                if (TempData["LotId"] != null)
                {
                    ViewData["LotId"] = TempData["LotId"];
                    TempData.Keep("LotId");
                }
                return View(grupo);
            }

            if (TempData["LotId"] == null)
            {
                ModelState.AddModelError(string.Empty, "LotId no encontrado en TempData.");
                return View(grupo);
            }

            int lotId = (int)TempData["LotId"];
            grupo.IdLote = lotId;

            var jsonContent = new StringContent(JsonSerializer.Serialize(grupo), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:7274/Grupos/Create", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetByLotId", new { id = grupo.IdLote });
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"API error: {errorResponse}");

            if (TempData["LotId"] != null)
            {
                ViewData["LotId"] = TempData["LotId"];
                TempData.Keep("LotId");
            }

            return View(grupo);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var response = await _httpClient.GetAsync($"https://localhost:7274/Grupos/Details?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var grupo = JsonSerializer.Deserialize<Grupos>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (grupo != null)
                {
                    return View(grupo);
                }
            }

            return NotFound();
        }

        
        public async Task<IActionResult> Edit(int id, Grupos grupo)
        {
            if (!ModelState.IsValid)
            {
                return View(grupo);
            }

            var jsonContent = new StringContent(JsonSerializer.Serialize(grupo), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"https://localhost:7274/Grupos/Edit/?id={id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetByLotId", new { lotId = grupo.IdLote });
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"API error: {errorResponse}");

            return View(grupo);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupo = await _httpClient.GetFromJsonAsync<Grupos>($"https://localhost:7274/Grupos/Details?id={id}");

            if (grupo == null)
            {
                return NotFound();
            }

            return View(grupo);
        }
        [HttpPost, ActionName("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7274/Grupos/Delete?id={id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetByLotId");
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"API error: {errorResponse}");
            return RedirectToAction("GetByLotId");
        }
    }
}
