using Microsoft.AspNetCore.Mvc;
using ProjectPOE_Prog3A.Models;
using System.Text;
using System.Text.Json;

public class ContractsController : Controller
{
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public ContractsController(IWebHostEnvironment environment, IHttpClientFactory httpClientFactory)
    {
        _environment = environment;
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("ContractsApi");

    private ContractStatus GetContractStatus(Contracts contract)
    {
        var today = DateTime.Today;
        if (contract.StartDate > today) return ContractStatus.Draft;
        if (contract.StartDate <= today && contract.EndDate >= today) return ContractStatus.Active;
        if (contract.EndDate < today) return ContractStatus.Expired;
        return ContractStatus.Draft;
    }

    public async Task<IActionResult> Index(string searchstring)
    {
        var httpClient = CreateClient();
        var response = await httpClient.GetAsync("api/contracts");

        if (!response.IsSuccessStatusCode)
            return Problem("Could not retrieve contracts from API");

        var json = await response.Content.ReadAsStringAsync();
        var contracts = JsonSerializer.Deserialize<List<Contracts>>(json, _jsonOptions) ?? new();

        if (!string.IsNullOrEmpty(searchstring))
        {
            searchstring = searchstring.ToLower();
            contracts = contracts.Where(c =>
                c.ContractName.ToLower().Contains(searchstring) ||
                c.StartDate.ToString().Contains(searchstring) ||
                c.EndDate.ToString().Contains(searchstring)
            ).ToList();
        }

        foreach (var c in contracts)
            c.ContractStatus = GetContractStatus(c);

        return View(contracts);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var httpClient = CreateClient();
        var response = await httpClient.GetAsync($"api/contracts/{id}");

        if (!response.IsSuccessStatusCode) return NotFound();

        var json = await response.Content.ReadAsStringAsync();
        var contract = JsonSerializer.Deserialize<Contracts>(json, _jsonOptions);

        if (contract == null) return NotFound();

        contract.ContractStatus = GetContractStatus(contract);
        ViewBag.Status = contract.ContractStatus;

        var filesResponse = await httpClient.GetAsync($"api/contracts/{id}/files");
        if (filesResponse.IsSuccessStatusCode)
        {
            var filesJson = await filesResponse.Content.ReadAsStringAsync();
            contract.Files = JsonSerializer.Deserialize<List<ContractFile>>(filesJson, _jsonOptions) ?? new();
        }
        else
        {
            contract.Files = new List<ContractFile>();
        }

        return View(contract);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("ContractName,ContractDescription,ContractType,StartDate,EndDate")] Contracts contract)
    {
        ModelState.Remove("clients");
        ModelState.Remove("Files");
        ModelState.Remove("ContractStatus");

        if (!ModelState.IsValid) return View(contract);

        var httpClient = CreateClient();

        var addContractDto = new
        {
            contract.ContractName,
            contract.ContractType,
            contract.ContractDescription,
            contract.StartDate,
            contract.EndDate
        };

        var json = JsonSerializer.Serialize(addContractDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("api/contracts", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to create contract.");
            return View(contract);
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var created = JsonSerializer.Deserialize<Contracts>(responseJson, _jsonOptions);

        return RedirectToAction(nameof(Details), new { id = created?.Id });
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var httpClient = CreateClient();
        var response = await httpClient.GetAsync($"api/contracts/{id}");

        if (!response.IsSuccessStatusCode) return NotFound();

        var json = await response.Content.ReadAsStringAsync();
        var contract = JsonSerializer.Deserialize<Contracts>(json, _jsonOptions);

        return View(contract);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Contracts contract)
    {
        if (id != contract.Id) return NotFound();
        if (!ModelState.IsValid) return View(contract);

        var httpClient = CreateClient();
        var json = JsonSerializer.Serialize(contract);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PutAsync($"api/contracts/{id}", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to update contract.");
            return View(contract);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var httpClient = CreateClient();
        var response = await httpClient.GetAsync($"api/contracts/{id}");

        if (!response.IsSuccessStatusCode) return NotFound();

        var json = await response.Content.ReadAsStringAsync();
        var contract = JsonSerializer.Deserialize<Contracts>(json, _jsonOptions);

        return View(contract);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var httpClient = CreateClient();
        var response = await httpClient.DeleteAsync($"api/contracts/{id}");

        if (!response.IsSuccessStatusCode)
            return Problem("Failed to delete contract.");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Upload(int contractId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select a file.";
            return RedirectToAction("Details", new { id = contractId });
        }

        if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "Only PDF files are allowed.";
            return RedirectToAction("Details", new { id = contractId });
        }

        var httpClient = CreateClient();
        var contractCheck = await httpClient.GetAsync($"api/contracts/{contractId}");
        if (!contractCheck.IsSuccessStatusCode)
            return NotFound();

        try
        {
            string folder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string fullPath = Path.Combine(folder, uniqueName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var contractFile = new ContractFile
            {
                FileName = file.FileName,
                FilePath = uniqueName,
                FileSize = file.Length,
                UploadedDate = DateTime.Now,
                ContractId = contractId
            };

            var json = JsonSerializer.Serialize(contractFile);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("api/contracts/files", content);
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Upload failed: " + ex.Message;
        }

        return RedirectToAction("Details", new { id = contractId });
    }

    public async Task<IActionResult> DownloadFile(int fileId, bool download = false)
    {
        var httpClient = CreateClient();
        var response = await httpClient.GetAsync($"api/contracts/files/{fileId}");
        if (!response.IsSuccessStatusCode) return NotFound();

        var json = await response.Content.ReadAsStringAsync();
        var file = JsonSerializer.Deserialize<ContractFile>(json, _jsonOptions);
        if (file == null) return NotFound();

        string path = Path.Combine(_environment.WebRootPath, "uploads", file.FilePath);
        if (!System.IO.File.Exists(path)) return NotFound();

        byte[] bytes = System.IO.File.ReadAllBytes(path);

        if (download)
            return File(bytes, "application/pdf", file.FileName);

        return File(bytes, "application/pdf");
    }
}