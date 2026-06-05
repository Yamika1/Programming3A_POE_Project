using Microsoft.EntityFrameworkCore;

public class ProjectPOE_Prog3AContext(DbContextOptions<ProjectPOE_Prog3AContext> options) : DbContext(options)
{
    public DbSet<ProjectPOE_Prog3A.Models.Client> Client { get; set; } = default!;
    public DbSet<ProjectPOE_Prog3A.Models.Contracts> Contracts { get; set; } = default!;
    public DbSet<ProjectPOE_Prog3A.Models.ServiceRequests> ServiceRequests { get; set; } = default!;
    public DbSet<ProjectPOE_Prog3A.Models.ContractFile> contractFiles { get; set; } 
}
