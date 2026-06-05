using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web_Api.Data;
using Web_Api.Models;

namespace Web_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public ExchangeRatesController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("{from}/{to}")]
        public IActionResult GetRate(string from, string to)
        {
            var rate = dbContext.ExchangeRates.FirstOrDefault(r =>
                r.FromCurrency == from.ToUpper() &&
                r.ToCurrency == to.ToUpper());

            if (rate == null) return NotFound();
            return Ok(rate);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(dbContext.ExchangeRates.ToList());
        }

        [HttpPost]
        public IActionResult AddRate([FromBody] ExchangeRate rate)
        {
            rate.FromCurrency = rate.FromCurrency.ToUpper();
            rate.ToCurrency = rate.ToCurrency.ToUpper();

            var existing = dbContext.ExchangeRates.FirstOrDefault(r =>
                r.FromCurrency == rate.FromCurrency &&
                r.ToCurrency == rate.ToCurrency);

            if (existing != null)
            {
                existing.Rate = rate.Rate;
                dbContext.SaveChanges();
                return Ok(existing);
            }

            dbContext.ExchangeRates.Add(rate);
            dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetRate),
                new { from = rate.FromCurrency, to = rate.ToCurrency }, rate);
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteRate(int id)
        {
            var rate = dbContext.ExchangeRates.Find(id);
            if (rate == null) return NotFound();
            dbContext.ExchangeRates.Remove(rate);
            dbContext.SaveChanges();
            return NoContent();
        }
    }
}