using System.Globalization;
using Microsoft.EntityFrameworkCore;
using DataFuse.Integration.Helpers;
using DataFuse.Adapters.EntityFramework.Tests.Domain;

namespace DataFuse.Adapters.EntityFramework.Tests
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(
             eb =>
             {
                 eb.ToTable("TCustomer");
                 eb.HasKey(b => b.Id);
                 eb.Property(b => b.Id).HasColumnName("CustomerId");
                 eb.Property(b => b.Name).HasColumnName("Customer_Name");
                 eb.Property(b => b.Code).HasColumnName("Customer_Code");
                 eb.HasOne(b => b.Communication).WithOne(c => c.Customer).HasForeignKey<Communication>(c => c.CustomerId);
                 eb.HasMany(b => b.Orders).WithOne(c => c.Customer).HasForeignKey(c => c.OrderId);
             });

            modelBuilder.Entity<Communication>(
             eb =>
             {
                 eb.ToTable("TCommunication");
                 eb.HasKey(b => b.CommunicationId);
                 eb.HasOne(b => b.Customer);
                 eb.Property(b => b.Phone);
                 eb.Property(b => b.Email);
             });

            modelBuilder.Entity<Address>(
             eb =>
             {
                 eb.ToTable("TAddress");
                 eb.HasKey(b => b.AddressId);
                 eb.Property(b => b.HouseNo);
                 eb.Property(b => b.Region);
                 eb.Property(b => b.City);
                 eb.Property(b => b.Country);
                 eb.Property(b => b.PostalCode).HasColumnName("Postcode");
                 eb.HasOne(b => b.Communication).WithOne(c => c.Address).HasForeignKey<Address>(c => c.CommunicationId);
             });

            modelBuilder.Entity<Order>(
             eb =>
             {
                 eb.ToTable("TOrder");
                 eb.HasKey(b => b.OrderId);
                 eb.Property(b => b.OrderNo);
                 eb.Property(b => b.Date).HasColumnName("OrderDate")
                                         .HasConversion(v => v.ToShortDateString(), s => s.IsNotNullOrEmpty() ? DateTime.ParseExact(s, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) : DateTime.MinValue);
                 eb.HasOne(b => b.Customer);
                 eb.HasMany(b => b.Items);
             });

            modelBuilder.Entity<OrderItem>(
             eb =>
             {
                 eb.ToTable("TOrderItem");
                 eb.HasKey(b => b.ItemId);
                 eb.Property(b => b.ItemId).HasColumnName("OrderItemId");
                 eb.Property(b => b.Cost);
                 eb.Property(b => b.Name);
                 eb.HasOne(b => b.Order).WithMany(c => c.Items).HasForeignKey(c => c.OrderId);
             });
        }

        public static class MyDbFunctions
        {
            public static DateTime StringToDate(string dateString) => throw new NotImplementedException();
        }
    }
}