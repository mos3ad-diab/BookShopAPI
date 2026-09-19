using BookShopAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookShopAPI.Data
{
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ApplicationUserOTP> ApplicationUserOTPs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
           
            // 2. English Seed Data

            // Authors Seed Data
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, Name = "George Orwell" },
                new Author { Id = 2, Name = "J.K. Rowling" },
                new Author { Id = 3, Name = "J.R.R. Tolkien" }
            );

            // Categories Seed Data
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Fiction" },
                new Category { Id = 2, Name = "Fantasy" },
                new Category { Id = 3, Name = "Sci-Fi" }
            );

            // Books Seed Data
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Name = "1984",
                    Price = 15.99m,
                    Discount = 2.00m,
                    Rate = 5,
                    Review = 320,
                    Year = new DateOnly(1949, 6, 8),
                    AuthorId = 1,
                    CategoryId = 1
                },
                new Book
                {
                    Id = 2,
                    Name = "Harry Potter and the Philosopher's Stone",
                    Price = 22.50m,
                    Discount = 3.50m,
                    Rate = 5,
                    Review = 450,
                    Year = new DateOnly(1997, 6, 26),
                    AuthorId = 2,
                    CategoryId = 2
                },
                new Book
                {
                    Id = 3,
                    Name = "The Hobbit",
                    Price = 18.00m,
                    Discount = 0.00m,
                    Rate = 4,
                    Review = 210,
                    Year = new DateOnly(1937, 9, 21),
                    AuthorId = 3,
                    CategoryId = 2
                }
            );
        }
    }
}
