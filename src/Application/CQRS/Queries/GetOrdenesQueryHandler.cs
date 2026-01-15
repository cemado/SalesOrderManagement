using MediatR;
using Application.DTOs;
using AutoMapper;
using Domain.Repositories;

namespace Application.CQRS.Queries
{
    /// <summary>
    /// Handler para obtener órdenes paginadas con filtros
    /// </summary>
    public class GetOrdenesQueryHandler : IRequestHandler<GetOrdenesQuery, PaginatedResponse<OrdenResponse>>
    {
        private readonly IOrdenRepository _repository;
        private readonly IMapper _mapper;

        public GetOrdenesQueryHandler(IOrdenRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<OrdenResponse>> Handle(GetOrdenesQuery request, CancellationToken cancellationToken)
        {
            var (ordenes, total) = await _repository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                request.ClienteFilter,
                request.FechaInicio,
                request.FechaFin);

            return new PaginatedResponse<OrdenResponse>
            {
                Items = _mapper.Map<List<OrdenResponse>>(ordenes),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = total
            };
        }
    }
}
