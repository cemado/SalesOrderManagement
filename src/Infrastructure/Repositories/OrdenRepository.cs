using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del repositorio de Órdenes usando Entity Framework Core
    /// </summary>
    public class OrdenRepository : IOrdenRepository
    {
        private readonly ApplicationDbContext _context;

        public OrdenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Orden?> GetByIdAsync(int id)
        {
            return await _context.Ordenes
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Orden>> GetAllAsync()
        {
            return await _context.Ordenes
                .Include(o => o.Detalles)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Orden> Ordenes, int Total)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? clienteFilter = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            var query = _context.Ordenes.Include(o => o.Detalles).AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(clienteFilter))
            {
                query = query.Where(o => o.Cliente.Contains(clienteFilter));
            }

            if (fechaInicio.HasValue)
            {
                query = query.Where(o => o.Fecha >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                query = query.Where(o => o.Fecha <= fechaFin.Value);
            }

            // Contar total antes de paginar
            var total = await query.CountAsync();

            // Paginar
            var ordenes = await query
                .OrderByDescending(o => o.Fecha)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (ordenes, total);
        }

        public async Task<bool> ExisteOrdenEnFechaAsync(string cliente, DateTime fecha)
        {
            var fechaInicio = fecha.Date;
            var fechaFin = fecha.Date.AddDays(1).AddTicks(-1);

            return await _context.Ordenes
                .AnyAsync(o => o.Cliente == cliente && 
                              o.Fecha >= fechaInicio && 
                              o.Fecha <= fechaFin);
        }

        public async Task<Orden> CreateAsync(Orden orden)
        {
            _context.Ordenes.Add(orden);
            return orden;
        }

        public async Task<Orden> UpdateAsync(Orden orden)
        {
            // ===== CLAVE: Attach permite actualizar entidad existente =====
            _context.Ordenes.Update(orden);
            return orden;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var orden = await GetByIdAsync(id);
            if (orden == null) return false;

            _context.Ordenes.Remove(orden);
            return true;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
