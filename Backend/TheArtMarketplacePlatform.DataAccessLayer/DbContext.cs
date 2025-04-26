using Microsoft.EntityFrameworkCore;
using TheArtMarketplacePlatform.Core.Entities;

namespace TheArtMarketplacePlatform.DataAccessLayer
{
    public class TheArtMarketplacePlatformDbContext : DbContext
    {
        // Your DbContext implementation here
        public TheArtMarketplacePlatformDbContext(DbContextOptions<TheArtMarketplacePlatformDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<CustomerProfile> CustomerProfiles { get; set; } = null!;
        public DbSet<ArtisanProfile> ArtisanProfiles { get; set; } = null!;
        public DbSet<DeliveryPartnerProfile> DeliveryPartnerProfiles { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public DbSet<ProductReview> ProductReviews { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderProduct> OrderProducts { get; set; } = null!;
        public DbSet<DeliveryStatusUpdate> DeliveryStatusUpdates { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure your entities and relationships here
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);
                entity.Property(u => u.PasswordSalt).IsRequired().HasMaxLength(256);
                entity.Property(u => u.Status).IsRequired().HasConversion<string>().HasDefaultValue(UserStatus.Active);
                entity.Property(u => u.Role).IsRequired().HasConversion<string>().HasDefaultValue(UserRole.Customer);
                entity.Property(u => u.IsDeleted).IsRequired().HasConversion<string>().HasDefaultValue(false);
                entity.Property(u => u.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
                entity.Property(u => u.UpdatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
                entity.Property(u => u.DeletedAt).IsRequired(false);
            });

            // ArtisanProfile entity configuration
            modelBuilder.Entity<ArtisanProfile>(entity =>
            {
                entity.HasKey(a => a.UserId);
                entity.Property(a => a.Bio).IsRequired().HasMaxLength(500);
                entity.Property(a => a.City).IsRequired().HasMaxLength(100);
                entity.Property(a => a.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(a => a.UpdatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(a => a.User)
                    .WithOne(u => u.ArtisanProfile)
                    .HasForeignKey<ArtisanProfile>(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(a => a.Products)
                    .WithOne(p => p.Artisan)
                    .HasForeignKey(p => p.ArtisanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CustomerProfile entity configuration
            modelBuilder.Entity<CustomerProfile>(entity =>
            {
                entity.HasKey(c => c.UserId);
                entity.Property(c => c.ShippingAddress).IsRequired().HasMaxLength(256);
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(c => c.UpdatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(c => c.User)
                    .WithOne(u => u.CustomerProfile)
                    .HasForeignKey<CustomerProfile>(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // DeliveryPartnerProfile entity configuration
            modelBuilder.Entity<DeliveryPartnerProfile>(entity =>
            {
                entity.HasKey(d => d.UserId);
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(c => c.UpdatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(d => d.User)
                    .WithOne(u => u.DeliveryPartnerProfile)
                    .HasForeignKey<DeliveryPartnerProfile>(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Product entity configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.ArtisanId).IsRequired();
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Description).IsRequired().HasMaxLength(500);
                entity.Property(p => p.Price).IsRequired().HasColumnType("decimal(10,2)");
                entity.Property(p => p.QuantityLeft).IsRequired().HasDefaultValue(0);

                // Check constraint for quantity left
                entity.ToTable(t => t.HasCheckConstraint("CK_Product_QuantityLeft", "QuantityLeft >= 0"));

                entity.Property(p => p.Status).IsRequired().HasConversion<string>(); //.HasDefaultValue(ProductStatus.OutOfStock);
                entity.Property(p => p.Availability).IsRequired().HasConversion<string>().HasDefaultValue(ProductAvailability.Available);

                // Query filter for IsAvailable
                // entity.HasQueryFilter(p => p.IsAvailable);

                entity.Property(p => p.IsDeleted).IsRequired().HasDefaultValue(false);

                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(p => p.UpdatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(p => p.DeletedAt).IsRequired(false);

                entity.HasOne(p => p.Artisan)
                    .WithMany(a => a.Products)
                    .HasForeignKey(p => p.ArtisanId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ProductCategory entity configuration
            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(pc => pc.Id);
                entity.Property(pc => pc.Name).IsRequired().HasMaxLength(100);
                entity.Property(pc => pc.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(pc => pc.UpdatedAt).HasDefaultValueSql("GETDATE()");
            });

            // ProductReview entity configuration
            modelBuilder.Entity<ProductReview>(entity =>
            {
                // composite key : only one review per customer per product
                entity.HasKey(pr => pr.Id);
                entity.Property(pr => pr.ProductId).IsRequired();
                entity.Property(pr => pr.CustomerComment).IsRequired().HasMaxLength(500);
                entity.Property(pr => pr.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(pr => pr.Rating).IsRequired();

                // Check constraint for rating value (1 to 5)
                entity.ToTable(t => t.HasCheckConstraint("CK_ProductReview_Rating", "Rating >= 1 AND Rating <= 5"));

                entity.Property(pr => pr.ArtisanResponse).HasMaxLength(500);
                entity.Property(pr => pr.UpdatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(pr => pr.Product)
                    .WithMany(p => p.ProductReviews)
                    .HasForeignKey(pr => pr.ProductId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(pr => pr.Customer)
                    .WithMany(c => c.ProductReviews)
                    .HasForeignKey(pr => pr.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Order entity configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.DeliveryPartnerName).IsRequired().HasMaxLength(100);
                entity.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(256);
                entity.Property(o => o.Status).IsRequired().HasConversion<string>().HasDefaultValue(OrderStatus.Pending);
                entity.Property(o => o.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(o => o.UpdatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(o => o.DeliveryPartner)
                    .WithMany(d => d.Orders)
                    .HasForeignKey(o => o.DeliveryPartnerId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // OrderProduct entity configuration
            modelBuilder.Entity<OrderProduct>(entity =>
            {
                entity.HasKey(op => op.Id);

                entity.Property(op => op.OrderId).IsRequired();
                entity.Property(op => op.ArtisanName).IsRequired().HasMaxLength(100);
                entity.Property(op => op.ProductName).IsRequired().HasMaxLength(100);
                entity.Property(op => op.ProductDescription).IsRequired().HasMaxLength(500);
                entity.Property(op => op.ProductPrice).IsRequired().HasColumnType("decimal(10,2)");
                entity.Property(op => op.Quantity).IsRequired().HasDefaultValue(1);

                entity.HasOne(op => op.Order)
                    .WithMany(o => o.OrderProducts)
                    .HasForeignKey(op => op.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(op => op.Product)
                    .WithMany(p => p.OrderProducts)
                    .HasForeignKey(op => op.ProductId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // DeliveryStatusUpdate entity configuration
            modelBuilder.Entity<DeliveryStatusUpdate>(entity =>
            {
                entity.HasKey(dsu => dsu.Id);
                entity.Property(dsu => dsu.OrderId).IsRequired();
                entity.Property(dsu => dsu.Status).IsRequired().HasConversion<string>().HasDefaultValue(DeliveryStatus.Pending);
                entity.Property(dsu => dsu.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(dsu => dsu.Order)
                    .WithMany(o => o.DeliveryStatusUpdates)
                    .HasForeignKey(dsu => dsu.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}