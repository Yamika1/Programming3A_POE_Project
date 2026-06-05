
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPOE_Prog3A.Models;
using System.Text;
using System.Text.Json;
using static ProjectPOE_Prog3A.Models.ConcreteObserver;

public class ServiceRequestsController : Controller
{
    private readonly ProjectPOE_Prog3AContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public ServiceRequestsController(ProjectPOE_Prog3AContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("ServiceRequestsApi");

    private ContractStatus GetContractStatus(Contracts contract)
    {
        var today = DateTime.Today;

        if (contract.ContractStatus == ContractStatus.OnHold)
            return ContractStatus.OnHold;

        if (contract.StartDate > today)
            return ContractStatus.Draft;

        if (contract.StartDate <= today && contract.EndDate >= today)
            return ContractStatus.Active;

        if (contract.EndDate < today)
            return ContractStatus.Expired;

        return ContractStatus.Draft;
    }

    public async Task<IActionResult> Index()
    {
        var httpClient = CreateClient();
        var response = await httpClient.GetAsync("api/servicerequests");

        if (!response.IsSuccessStatusCode)
            return Problem("Could not retrieve service requests from API");

        var json = await response.Content.ReadAsStringAsync();
        var serviceRequests = JsonSerializer.Deserialize<List<ServiceRequests>>(json, _jsonOptions) ?? new();

        return View(serviceRequests);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var httpClient = CreateClient();
        var response = await httpClient.GetAsync($"api/servicerequests/{id}");

        if (!response.IsSuccessStatusCode) return NotFound();

        var json = await response.Content.ReadAsStringAsync();
        var serviceRequest = JsonSerializer.Deserialize<ServiceRequests>(json, _jsonOptions);

        return View(serviceRequest);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceRequests serviceRequests)
    {
        ModelState.Remove("Contract");

        if (!ModelState.IsValid)
            return View(serviceRequests);

        var httpClient = CreateClient();


        var contractResponse = await httpClient.GetAsync($"api/contracts/{serviceRequests.ContractId}");

        if (!contractResponse.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Invalid contract, contract has the status of draft");
            return View(serviceRequests);
        }

        var contractJson = await contractResponse.Content.ReadAsStringAsync();
        var contract = JsonSerializer.Deserialize<Contracts>(contractJson, _jsonOptions);

        if (contract == null)
        {
            ModelState.AddModelError("", "Contract not found.");
            return View(serviceRequests);
        }

        contract.ContractStatus = GetContractStatus(contract);

        if (contract.ContractStatus == ContractStatus.Draft ||
            contract.ContractStatus == ContractStatus.OnHold)
        {
            ModelState.AddModelError("",
                "Cannot create service request because contract is Draft or On Hold.");
            return View(serviceRequests);
        }

        var addRequestDto = new
        {
            serviceRequests.ServiceStatus,
            serviceRequests.RequestDescription,
            serviceRequests.ContractCost,
            serviceRequests.RequestDate,
            serviceRequests.RequestTypes
        };

        var json = JsonSerializer.Serialize(addRequestDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("api/servicerequests", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to create service request.");
            return View(serviceRequests);
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var created = JsonSerializer.Deserialize<ServiceRequests>(responseJson, _jsonOptions);

        var notificationSystem = new NotificationSystem();
        notificationSystem.EnableNotifications(new Email());
        notificationSystem.EnableNotifications(new SMS());
        notificationSystem.NotifyObservers(
            $"Service Request #{created?.Id} created. " +
            $"Contract Status: {contract.ContractStatus}"
        );

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var httpClient = CreateClient();
        var response = await httpClient.GetAsync($"api/servicerequests/{id}");

        if (!response.IsSuccessStatusCode) return NotFound();

        var json = await response.Content.ReadAsStringAsync();
        var serviceRequest = JsonSerializer.Deserialize<ServiceRequests>(json, _jsonOptions);

        return View(serviceRequest);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,ServiceStatus,ContractCost,RequestTypes,RequestDate,RequestDescription")] ServiceRequests serviceRequests)
    {
        if (id != serviceRequests.Id) return NotFound();
        ModelState.Remove("Contract");
        if (!ModelState.IsValid) return View(serviceRequests);

        var httpClient = CreateClient();
        var json = JsonSerializer.Serialize(serviceRequests);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PutAsync($"api/servicerequests/{id}", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to update service request.");
            return View(serviceRequests);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var httpClient = CreateClient();
        var response = await httpClient.GetAsync($"api/servicerequests/{id}");

        if (!response.IsSuccessStatusCode) return NotFound();

        var json = await response.Content.ReadAsStringAsync();
        var serviceRequest = JsonSerializer.Deserialize<ServiceRequests>(json, _jsonOptions);

        return View(serviceRequest);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var httpClient = CreateClient();
        var response = await httpClient.DeleteAsync($"api/servicerequests/{id}");

        if (!response.IsSuccessStatusCode)
            return Problem("Failed to delete service request.");

        return RedirectToAction(nameof(Index));
    }

   
}
