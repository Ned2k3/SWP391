﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProjectSWP391.Models.Entity
{
    public partial class swp391Context : DbContext
    {
        public swp391Context()
        {
        }

        public swp391Context(DbContextOptions<swp391Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Blog> Blogs { get; set; } = null!;
        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public virtual DbSet<Service> Services { get; set; } = null!;
        public virtual DbSet<ServiceCategory> ServiceCategories { get; set; } = null!;
        public virtual DbSet<Shift> Shifts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server =(local); database = swp391;uid=sa;pwd=123456");
            }
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

                entity.Property(e => e.BookingId).ValueGeneratedNever();

                entity.Property(e => e.Content).HasMaxLength(50);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.BookingCustomers)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Booking_Account1");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.BookingEmployees)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Booking_Account");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK_Booking_Service");

                entity.HasOne(d => d.Shift)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.ShiftId)
                    .HasConstraintName("FK_Booking_Shift");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedback");

                entity.Property(e => e.Content).HasMaxLength(500);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_Feedback_Account");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_Feedback_Product");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK_Feedback_Service");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).ValueGeneratedOnAdd();

                entity.Property(e => e.OrderDate).HasColumnType("date");

                entity.HasOne(d => d.OrderNavigation)
                    .WithOne(p => p.Order)
                    .HasForeignKey<Order>(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Account");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Product");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Image).HasMaxLength(500);

                entity.Property(e => e.PcategoryId).HasColumnName("PCategoryId");

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.ProductName).HasMaxLength(100);

                entity.HasOne(d => d.Pcategory)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.PcategoryId)
                    .HasConstraintName("FK_Product_Product_Category");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(e => e.PcategoryId);

                entity.ToTable("Product_Category");

                entity.Property(e => e.PcategoryId)
                    .ValueGeneratedNever()
                    .HasColumnName("PCategoryId");

                entity.Property(e => e.PcategoryName)
                    .HasMaxLength(50)
                    .HasColumnName("PCategoryName");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("Service");

                entity.Property(e => e.Description).HasMaxLength(200);

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

            modelBuilder.Entity<Shift>(entity =>
            {
                entity.ToTable("Shift");

                entity.Property(e => e.ShiftDay).HasColumnType("date");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
