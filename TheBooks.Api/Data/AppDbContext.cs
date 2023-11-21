using Ghak.libraries.AppBase.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheBooks.Api.Model;

namespace TheBooks.Api.Data;

public class AppDbContext: IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<AppUserToken> AppUserTokens { get; set; }
    public DbSet<BookCollection> BookCollections { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ActivateModelSoftDelete();
        base.OnModelCreating(modelBuilder);
    }
}