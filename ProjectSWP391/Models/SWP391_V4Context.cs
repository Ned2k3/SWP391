using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProjectSWP391.Models
{
    public partial class SWP391_V4Context : DbContext
    {
        public SWP391_V4Context()
        {
        }

        public SWP391_V4Context(DbContextOptions<SWP391_V4Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Blog> Blogs { get; set; } = null!;
        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<IsWorking> IsWorkings { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public virtual DbSet<Service> Services { get; set; } = null!;
        public virtual DbSet<ServiceCategory> ServiceCategories { get; set; } = null!;
        public virtual DbSet<ServiceList> ServiceLists { get; set; } = null!;
        public virtual DbSet<ServiceMaterial> ServiceMaterials { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("MyCnn"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(50);

                entity.Property(e => e.Image).HasMaxLength(500);

                entity.Property(e => e.Password).HasMaxLength(50);
            });

            modelBuilder.Entity<Blog>(entity =>
            {
                entity.ToTable("Blog");

                entity.Property(e => e.BlogDate).HasColumnType("datetime");

                entity.Property(e => e.Content).HasColumnType("text");

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Blog_Account");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking");

                entity.Property(e => e.BookingDate).HasColumnType("date");

                entity.Property(e => e.Content).HasMaxLength(300);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.BookingCustomers)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Booking_Account1");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.BookingEmployees)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Booking_Account");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedback");

                entity.Property(e => e.FeedbackId).ValueGeneratedOnAdd();

                entity.Property(e => e.Content).HasMaxLength(500);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_Feedback_Account");

                entity.HasOne(d => d.FeedbackNavigation)
                    .WithOne(p => p.Feedback)
                    .HasForeignKey<Feedback>(d => d.FeedbackId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Feedback_Product");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK_Feedback_Service");
            });

            modelBuilder.Entity<IsWorking>(entity =>
            {
                entity.HasKey(e => e.WorkingId);

                entity.ToTable("IsWorking");

                entity.Property(e => e.WorkingId).ValueGeneratedNever();

                entity.Property(e => e.WorkingDay).HasColumnType("date");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.IsWorkings)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IsWorking_Account");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderDate).HasColumnType("date");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Account1");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetail");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__Order__5629CD9C");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__Produ__571DF1D5");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Image).HasMaxLength(300);

                entity.Property(e => e.PcategoryId).HasColumnName("PCategoryId");

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.ProductName).HasMaxLength(100);

                entity.HasOne(d => d.Pcategory)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.PcategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Product_Category");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(e => e.PcategoryId);

                entity.ToTable("Product_Category");

                entity.Property(e => e.PcategoryId).HasColumnName("PCategoryId");

                entity.Property(e => e.PcategoryName)
                    .HasMaxLength(100)
                    .HasColumnName("PCategoryName");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("Service");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Image).HasMaxLength(300);

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.ScategoryId).HasColumnName("SCategoryId");

                entity.Property(e => e.ServiceName).HasMaxLength(50);

                entity.HasOne(d => d.Scategory)
                    .WithMany(p => p.Services)
                    .HasForeignKey(d => d.ScategoryId)
                    .HasConstraintName("FK_Service_Service_Category");
            });

            modelBuilder.Entity<ServiceCategory>(entity =>
            {
                entity.HasKey(e => e.ScategoryId);

                entity.ToTable("Service_Category");

                entity.Property(e => e.ScategoryId).HasColumnName("SCategoryId");

                entity.Property(e => e.ScategoryName)
                    .HasMaxLength(50)
                    .HasColumnName("SCategoryName");
            });

            modelBuilder.Entity<ServiceList>(entity =>
            {
                entity.ToTable("ServiceList");

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.ServiceLists)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServiceList_Booking");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.ServiceLists)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServiceList_Service");
            });

            modelBuilder.Entity<ServiceMaterial>(entity =>
            {
                entity.HasKey(e => e.MaterialId);

                entity.ToTable("Service_Material");

                entity.Property(e => e.MaterialId).HasColumnName("Material_Id");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

                entity.Property(e => e.Image).HasMaxLength(250);

                entity.Property(e => e.MaterialName)
                    .HasMaxLength(50)
                    .HasColumnName("Material_Name");

                entity.Property(e => e.MaterialType)
                    .HasMaxLength(50)
                    .HasColumnName("Material_Type");

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.Suppiler).HasMaxLength(50);

                entity.Property(e => e.Unit).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.ServiceMaterials)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK_Service_Material_Service");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
