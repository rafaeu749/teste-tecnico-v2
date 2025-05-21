using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.ApiService.Models;

namespace Thunders.TechTest.ApiService.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TollPlaza> TollPlazas { get; set; }
        public DbSet<TollRegister> TollRegisters { get; set; }
        public DbSet<TollReport> TollReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TollPlaza>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).IsRequired();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.City).IsRequired();
            });

            modelBuilder.Entity<TollRegister>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()")
                    .IsRequired();

                entity.Property(e => e.RegisteredAt).IsRequired();

                entity.Property(e => e.AmountPaid)
                    .HasPrecision(precision: 18, scale: 2)
                    .IsRequired();

                entity.Property(e => e.VehicleType).IsRequired();

                entity.Property(e => e.TollPlazaId).IsRequired();

                entity.HasOne<TollPlaza>()
                    .WithMany()
                    .HasForeignKey(e => e.TollPlazaId)
                    .OnDelete(DeleteBehavior.Cascade); // TODO: Set the delete behavior as per requirements
            });

            modelBuilder.Entity<TollReport>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .IsRequired();

                entity.Property(e => e.CreatedAt).IsRequired();

                entity.Property(e => e.ProcessedAt);

                entity.Property(e => e.ReportType).IsRequired();

                entity.Ignore(e => e.Data);

                entity.Property(e => e.SerializedData)
                    .HasColumnName("Data")
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false);
            });


        }
    }
}
