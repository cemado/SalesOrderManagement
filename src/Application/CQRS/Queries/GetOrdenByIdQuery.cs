using MediatR;
using Application.DTOs;

namespace Application.CQRS.Queries
{
    /// <summary>
    /// Query para obtener una orden por ID
    /// </summary>
    public class GetOrdenByIdQuery : IRequest<OrdenResponse>
    {
        public int OrdenId { get; set; }
    }
}
