using CarPartsSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarPartsSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Part> Parts { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Slide> Slides { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<BookingPostponeRequest> BookingPostponeRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.User)
                .WithMany(u => u.Purchases)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Staff)
                .WithMany()
                .HasForeignKey(p => p.StaffId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Purchase)
                .WithMany(p => p.PurchaseItems)
                .HasForeignKey(pi => pi.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Part)
                .WithMany(p => p.PurchaseItems)
                .HasForeignKey(pi => pi.PartId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany()
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Part)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.PartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(pt => pt.Purchase)
                .WithMany(p => p.PaymentTransactions)
                .HasForeignKey(pt => pt.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(pt => pt.PaymentMethod)
                .WithMany(pm => pm.PaymentTransactions)
                .HasForeignKey(pt => pt.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(pt => pt.User)
                .WithMany(u => u.PaymentTransactions)
                .HasForeignKey(pt => pt.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
