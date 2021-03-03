using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace DemoECummerApp.Models
{
    public partial class DemoDatabaseContext : DbContext
    {
        public virtual DbSet<Customers> Customers { get; set; }

        public DemoDatabaseContext(DbContextOptions<DemoDatabaseContext> options)
            : base(options){ }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-D1SN2MM\\SQLEXPRESS;Database=DemoDatabase;Trusted_Connection=True;");
            }       
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Customers>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Dob)
                    .HasColumnType("datetime")
                    .HasColumnName("dob");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(110)
                    .HasColumnName("email");

                entity.Property(e => e.Fax)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("fax");

                entity.Property(e => e.Firstname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("firstname");

                entity.Property(e => e.Gender)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("gender")
                    .IsFixedLength(true);

                entity.Property(e => e.Lastname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("lastname")
                    .IsFixedLength(true);

                entity.Property(e => e.Mainaddressid).HasColumnName("mainaddressid");

                entity.Property(e => e.Newsletteropted).HasColumnName("newsletteropted");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("password");

                entity.Property(e => e.Telephone)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("telephone");
            });
        }
    }
}
