using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LibraryAPI.Models
{
    public partial class LibraryContext : DbContext
    {
        public LibraryContext()
        {
        }

        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Books> Books { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UsersBooks> UsersBooks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DESKTOP-P4NO1RB\\SQLEXPRESS;Database=Library;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Books>(entity =>
            {
                entity.Property(e => e.Author)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("UQ__Users__AB6E61646EE675FD")
                    .IsUnique();

                entity.HasIndex(e => e.Login)
                    .HasName("UQ__Users__5E55825BA55BDA2E")
                    .IsUnique();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(10);

                entity.Property(e => e.Firstname)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Lastname)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Pwd)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasColumnName("role")
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<UsersBooks>(entity =>
            {
                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("smalldatetime");

                entity.HasOne(d => d.Books)
                    .WithMany(p => p.UsersBooks)
                    .HasForeignKey(d => d.BooksId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BooksId_BooksId");

                entity.HasOne(d => d.Users)
                    .WithMany(p => p.UsersBooks)
                    .HasForeignKey(d => d.UsersId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UsersId_UsersId");
            });
        }
    }
}
