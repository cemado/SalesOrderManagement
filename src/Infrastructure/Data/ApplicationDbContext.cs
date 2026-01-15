using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Data
{
    /// <summary>
    /// DbContext principal de la aplicación
    /// Gestiona las entidades y las operaciones de base de datos
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Orden> Ordenes { get; set; } = null!;
        public DbSet<DetalleOrden> DetalleOrdenes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Orden
            modelBuilder.Entity<Orden>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Fecha)
                    .IsRequired();

                entity.Property(e => e.Cliente)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Total)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Pendiente");

                // Relación uno-a-muchos
                entity.HasMany(o => o.Detalles)
                    .WithOne(d => d.Orden)
                    .HasForeignKey(d => d.OrdenId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("Ordenes");

                // Índice único: cliente + fecha
                entity.HasIndex(o => new { o.Cliente, o.Fecha })
                    .IsUnique()
                    .HasName("IX_Ordenes_Cliente_Fecha");
            });

            // Configuración de DetalleOrden
            modelBuilder.Entity<DetalleOrden>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.OrdenId)
                    .IsRequired();

                entity.Property(e => e.Producto)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Cantidad)
                    .IsRequired();

                entity.Property(e => e.PrecioUnitario)
                    .HasColumnType("decimal(18,2)");

                entity.ToTable("DetalleOrdenes");

                // Índice para búsquedas frecuentes
                entity.HasIndex(d => d.OrdenId)
                    .HasName("IX_DetalleOrdenes_OrdenId");
            });
        }
    }
}
