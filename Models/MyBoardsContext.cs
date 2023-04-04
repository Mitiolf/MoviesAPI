using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Filmy.Models
{
    public class MyBoardsContext : DbContext
    {
        public MyBoardsContext(DbContextOptions<MyBoardsContext> options): base(options)
        {
        }


        public DbSet<MovieModel> Movies { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserCollectionModel> UsersColections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MovieModel>(eb => {
                eb.HasKey("Id");
                eb.HasMany(m => m.UserMovies)
                .WithOne(c => c.Movie)
                .HasForeignKey("MovieId");
                eb.HasIndex(e => e.Title).IsUnique();
            });
            modelBuilder.Entity<UserCollectionModel>(eb => {
                eb.HasKey(c => new { c.MovieId, c.UserId });
            });
            modelBuilder.Entity<UserModel>(eb => {
                eb.HasKey("Id");
                eb.HasMany(u => u.UserMovies)
                .WithOne(c => c.User)
                .HasForeignKey("UserId");
                eb.HasIndex(e => e.Username).IsUnique();
            });
            }

    }
}
