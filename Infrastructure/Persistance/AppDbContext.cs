using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistance
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<NotificationLog> NotificationLogs { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Channel> Channels { get; set; }
        

        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .Property(u => u.Body)
                .HasMaxLength(1000);

            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(250);

            modelBuilder.Entity<User>()
                .Property(u => u.Phone)
                .HasMaxLength(10);

            modelBuilder.Entity<NotificationLog>()
                .Property(n => n.ErrorMessage)
                .HasMaxLength(1000);
        }
    }

}
