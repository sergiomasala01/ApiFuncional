using APIFuncional.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APIFuncional.Data
{
    public class ApiDbContext : IdentityDbContext //Herdar após o identity 
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) 
            : base(options)
        {
        }
        public DbSet<Produto> Produtos { get; set; }
    }
}

