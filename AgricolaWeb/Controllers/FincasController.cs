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
    public class FincasController : Controller
    {
        private readonly HttpClient _httpClient;

        public FincasController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("https://localhost:7274/Fincas/GetAll");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var fincas = JsonSerializer.Deserialize<List<Finca>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(fincas);
            }

            return View(new List<Finca>()); // Devolver una lista vacía en caso de error
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"https://localhost:7274/Fincas/Details?id={id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var finca = JsonSerializer.Deserialize<Finca>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(finca);
            }

            return NotFound();
        }

        public async Task<IActionResult> Create(Finca finca)
        {
            if (!ModelState.IsValid)
            {
                return View(finca); // Devuelve la vista con los datos ingresados si hay errores de validación
            }

            var jsonContent = new StringContent(JsonSerializer.Serialize(finca), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:7274/Fincas/Create", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Finca>(jsonString); 
                return RedirectToAction("Index");
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"API error: {errorResponse}");

            return View(finca); 
        }




        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7274/Fincas/Details?id={id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var finca = JsonSerializer.Deserialize<Finca>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(finca);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Finca finca)
        {
            if (!ModelState.IsValid)
            {
                return View(finca);
            }

            var jsonContent = new StringContent(JsonSerializer.Serialize(finca), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"https://localhost:7274/Fincas/Edit?id={id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            var errorResponse = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"API error: {errorResponse}");

            return View(finca);
        }

        
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finca = await _httpClient.GetFromJsonAsync<Finca>($"https://localhost:7274/Fincas/Details?id={id}");

            if (finca == null)
            {
                return NotFound();
            }

            return View(finca);
        }

        
        [HttpPost, ActionName("Delete")]
        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7274/Fincas/Delete?id={id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"API error: {errorResponse}");
            return RedirectToAction("Index");
        }

    }
}
