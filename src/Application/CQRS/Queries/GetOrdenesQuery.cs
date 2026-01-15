using MediatR;
using Application.DTOs;

namespace Application.CQRS.Queries
{
    /// <summary>
    /// Query para obtener órdenes con paginación y filtros
    /// </summary>
    public class GetOrdenesQuery : IRequest<PaginatedResponse<OrdenResponse>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? ClienteFilter { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }

    /// <summary>
    /// Response paginado genérico
    /// </summary>
    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
