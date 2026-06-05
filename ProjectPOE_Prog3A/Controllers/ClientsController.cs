
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPOE_Prog3A.Models;
using System.Text;
using System.Text.Json;

public class ClientsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public ClientsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("ClientsApi");

    public async Task<IActionResult> Index(string searchstring)
    {
        var httpClient = CreateClient();
        var response = await httpClient.GetAsync("api/clients");

        if (!response.IsSuccessStatusCode)
            return Problem("Could not retrieve clients from API");

        var json = await response.Content.ReadAsStringAsync();
        var clients = JsonSerializer.Deserialize<List<Client>>(json, _jsonOptions) ?? new();

        if (!string.IsNullOrEmpty(searchstring))
        {
            searchstring = searchstring.ToLower();
            clients = clients.Where(c =>
                c.ClientFirstName.ToLower().Contains(searchstring) ||
                c.ClientLastName.ToLower().Contains(searchstring)
            ).ToList();
        }

        return View(clients);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var httpClient = CreateClient();
        var response = await httpClient.GetAsync($"api/clients/{id}");

        if (!response.IsSuccessStatusCode) return NotFound();

        var json = await response.Content.ReadAsStringAsync();
        var clientModel = JsonSerializer.Deserialize<Client>(json, _jsonOptions);

        return View(clientModel);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("ClientFirstName,ClientLastName,Region,ContactNumber,EmailAddress")] Client clientModel)
    {
        if (!ModelState.IsValid) return View(clientModel);

        var httpClient = CreateClient();
        var json = JsonSerializer.Serialize(clientModel);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("api/clients", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to create client.");
            return View(clientModel);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var httpClient = CreateClient();
        var response = await httpClient.GetAsync($"api/clients/{id}");

        if (!response.IsSuccessStatusCode) return NotFound();

        var json = await response.Content.ReadAsStringAsync();
        var clientModel = JsonSerializer.Deserialize<Client>(json, _jsonOptions);

        return View(clientModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("ClientId,ClientFirstName,ClientLastName,Region,ContactNumber,EmailAddress")] Client clientModel)
    {
        if (id != clientModel.ClientId) return NotFound();
        if (!ModelState.IsValid) return View(clientModel);

        var httpClient = CreateClient();
        var json = JsonSerializer.Serialize(clientModel);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PutAsync($"api/clients/{id}", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to update client.");
            return View(clientModel);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var httpClient = CreateClient();
        var response = await httpClient.GetAsync($"api/clients/{id}");

        if (!response.IsSuccessStatusCode) return NotFound();

        var json = await response.Content.ReadAsStringAsync();
        var clientModel = JsonSerializer.Deserialize<Client>(json, _jsonOptions);

        return View(clientModel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var httpClient = CreateClient();
        var response = await httpClient.DeleteAsync($"api/clients/{id}");

        if (!response.IsSuccessStatusCode)
            return Problem("Failed to delete client.");

        return RedirectToAction(nameof(Index));
    }
}
