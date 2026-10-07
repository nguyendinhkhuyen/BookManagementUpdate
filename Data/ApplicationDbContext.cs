using BookManagerApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BookManagerApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
    }
}