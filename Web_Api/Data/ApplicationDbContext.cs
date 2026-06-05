using Microsoft.EntityFrameworkCore;
using Web_Api.Models;
using Web_Api.Models.Entities;

namespace Web_Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Contracts> Contracts { get; set; }

        public DbSet<Client> Clients { get; set; }
        public DbSet<ServiceRequests> ServiceRequests { get; set; }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contracts>().ToTable("Contracts");
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<ServiceRequests>().ToTable("ServiceRequests");
        }
    }
}

