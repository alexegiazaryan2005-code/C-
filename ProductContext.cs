using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ProductManufacturingSystem
{
    public class ProductContext : DbContext
    {
        public ProductContext() : base("name=ProductDB") { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Workshop> Workshops { get; set; }
        public DbSet<WorkshopTime> WorkshopTimes { get; set; }
    }

    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    [Table("Products")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string ProductName { get; set; }

        public string Article { get; set; }

        public decimal MinPartnerPrice { get; set; }

        public string MainMaterial { get; set; }

        public virtual ICollection<WorkshopTime> WorkshopTimes { get; set; }

        public int GetTotalManufacturingTime()
        {
            int totalTime = 0;
            if (WorkshopTimes != null)
            {
                foreach (var wt in WorkshopTimes)
                {
                    totalTime += wt.TimeInHours;
                }
            }
            return totalTime;
        }
    }

    [Table("Workshops")]
    public class Workshop
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string WorkshopName { get; set; }
    }

    [Table("WorkshopTimes")]
    public class WorkshopTime
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int WorkshopId { get; set; }
        public virtual Workshop Workshop { get; set; }

        public int TimeInHours { get; set; }
    }
}