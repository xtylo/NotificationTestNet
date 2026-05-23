using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistance.Seeders
{
    internal static class CategorySeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Categories.Any())
                return;

            context.Categories.AddRange(
                new Category { Name = "Sports" },
                new Category { Name = "Finance" },
                new Category { Name = "Movies" }
            );

            context.SaveChanges();
        }
    }
}
