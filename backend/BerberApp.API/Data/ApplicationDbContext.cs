using BerberApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BerberApp.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet'ler
        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Service> Services { get; set; }

        public DbSet<WorkingHour> WorkingHours { get; set; }
        public DbSet<Barber> Barbers { get; set; }  // Yeni eklendi
        public DbSet<AppointmentService> AppointmentServices { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.Property(a => a.AppointmentDate)
                    .HasColumnType("datetime2")
                    .HasConversion(
                        v => v.Kind == DateTimeKind.Unspecified
                            ? DateTime.SpecifyKind(v, DateTimeKind.Utc)
                            : v.ToUniversalTime(),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

                entity.Property(a => a.CreatedAt)
                    .HasColumnType("datetime2")
                    .HasDefaultValueSql("GETUTCDATE()")
                    .ValueGeneratedOnAdd();

                entity.Property(a => a.UpdatedAt)
                    .HasColumnType("datetime2")
                    .ValueGeneratedOnUpdate();
            });

            modelBuilder.Entity<Barber>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.FullName).IsRequired().HasMaxLength(100);
                entity.Property(b => b.Email).HasMaxLength(100);
                entity.Property(b => b.Phone).HasMaxLength(20);
                entity.Property(b => b.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Specialties için JSON serileştirme (SQLite veya PostgreSQL için)
                entity.Property(b => b.Specialties)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                entity.HasMany(b => b.WorkingHours)
              .WithOne(wh => wh.Barber)
              .HasForeignKey(wh => wh.BarberId)
              .OnDelete(DeleteBehavior.Cascade);
            });

            // Burada fluent API ile ek konfigurasyonlar yapılabilir

            // Örnek: Appointment için Status enum string olarak saklanacak
            modelBuilder.Entity<Appointment>()
                .Property(a => a.Status)
                .HasConversion<string>();

            // Kullanıcı email benzersiz olsun örnek
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // İlişkiler (gerekirse)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AppointmentService>()
    .HasKey(x => new { x.AppointmentId, x.ServiceId });

            modelBuilder.Entity<AppointmentService>()
                .HasOne(x => x.Appointment)
                .WithMany(a => a.AppointmentServices)
                .HasForeignKey(x => x.AppointmentId);

            modelBuilder.Entity<AppointmentService>()
                .HasOne(x => x.Service)
                .WithMany(s => s.AppointmentServices)
                .HasForeignKey(x => x.ServiceId);

        }
    }
}
