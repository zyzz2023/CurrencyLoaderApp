using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<CurrencyRate> CurrencyRates { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrencyRate>()
                .HasKey(cr => new { cr.CurrencyCode, cr.Date });

            modelBuilder.Entity<CurrencyRate>()
                .HasIndex(cr => cr.Date);

            modelBuilder.Entity<CurrencyRate>()
                .HasIndex(cr => new { cr.CurrencyCode, cr.Date })
                .IsUnique();

            modelBuilder.Entity<CurrencyRate>()
                .Property(cr => cr.Value)
                .HasPrecision(18, 4);

            modelBuilder.Entity<CurrencyRate>()
                .Property(cr => cr.CurrencyCode)
                .HasMaxLength(5);

            modelBuilder.Entity<CurrencyRate>()
                .Property(cr => cr.CurrencyName)
                .HasMaxLength(100);
        }
    }
}
