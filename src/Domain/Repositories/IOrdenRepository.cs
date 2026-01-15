using Domain.Entities;

namespace Domain.Repositories
{
    /// <summary>
    /// Interfaz de repositorio para acceso a datos de órdenes
    /// Define el contrato que debe implementar la capa de infraestructura
    /// </summary>
    public interface IOrdenRepository
    {
        /// <summary>
        /// Obtiene una orden por su ID
        /// </summary>
        Task<Orden?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene todas las órdenes
        /// </summary>
        Task<IEnumerable<Orden>> GetAllAsync();

        /// <summary>
        /// Obtiene órdenes con paginación y filtros
        /// </summary>
        Task<(IEnumerable<Orden> Ordenes, int Total)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? clienteFilter = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null);

        /// <summary>
        /// Verifica si existe una orden del cliente en la fecha especificada
        /// </summary>
        Task<bool> ExisteOrdenEnFechaAsync(string cliente, DateTime fecha);

        /// <summary>
        /// Crea una nueva orden
        /// </summary>
        Task<Orden> CreateAsync(Orden orden);

        /// <summary>
        /// Actualiza una orden existente
        /// </summary>
        Task<Orden> UpdateAsync(Orden orden);

        /// <summary>
        /// Elimina una orden
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Guarda los cambios en la base de datos
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
