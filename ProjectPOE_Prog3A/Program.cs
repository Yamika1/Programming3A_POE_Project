using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ProjectPOE_Prog3AContext")
    ?? throw new InvalidOperationException("Connection string 'ProjectPOE_Prog3AContext' not found.");

builder.Services.AddDbContext<ProjectPOE_Prog3AContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("ClientsApi", client =>
{
    client.BaseAddress = new Uri("http://glms-backend-api:8080/");
});

builder.Services.AddHttpClient("ContractsApi", client =>
{
    client.BaseAddress = new Uri("http://glms-backend-api:8080/");
});

builder.Services.AddHttpClient("ServiceRequestsApi", client =>
{
    client.BaseAddress = new Uri("http://glms-backend-api:8080/");
});

builder.Services.AddHttpClient("CurrencyApi", client =>
{
    client.BaseAddress = new Uri("https://v6.exchangerate-api.com/v6/");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();