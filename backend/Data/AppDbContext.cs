using Microsoft.EntityFrameworkCore;
using PaymentApi.Models;

namespace PaymentApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<PaymentTransaction> PaymentTransactions =>
        Set<PaymentTransaction>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Token)
                .IsUnique();

            entity.Property(x => x.TerminalNo)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.RedirectUrl)
                .IsRequired()
                .HasMaxLength(2048);

            entity.Property(x => x.ReservationNumber)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(x => x.Status)
                .IsRequired();

            entity.Property(x => x.Rrn)
                .HasMaxLength(12);

            entity.Property(x => x.CreatedAt)
                .IsRequired();
        });
    }
}