using Microsoft.EntityFrameworkCore;
using Web_Api.Data;
using Web_Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers()
    .AddNewtonsoftJson();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();

        if (!db.ExchangeRates.Any())
        {
            db.ExchangeRates.AddRange(
                new ExchangeRate { FromCurrency = "USD", ToCurrency = "ZAR", Rate = 18.50 },
                new ExchangeRate { FromCurrency = "ZAR", ToCurrency = "USD", Rate = 0.054 },
                new ExchangeRate { FromCurrency = "USD", ToCurrency = "EUR", Rate = 0.92 },
                new ExchangeRate { FromCurrency = "EUR", ToCurrency = "USD", Rate = 1.09 },
                new ExchangeRate { FromCurrency = "ZAR", ToCurrency = "GBP", Rate = 0.043 },
                new ExchangeRate { FromCurrency = "USD", ToCurrency = "GBP", Rate = 0.79 },
                new ExchangeRate { FromCurrency = "EUR", ToCurrency = "ZAR", Rate = 20.10 },
                new ExchangeRate { FromCurrency = "ZAR", ToCurrency = "EUR", Rate = 0.050 }
            );
            db.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration failed: {ex.Message}");
    }
}

app.Run();

public partial class Program { }