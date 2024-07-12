using Microsoft.EntityFrameworkCore;
using MpesaApi.Models;

namespace MpesaApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<AccessTokenResponse> AccessTokenResponses { get; set; }
}
