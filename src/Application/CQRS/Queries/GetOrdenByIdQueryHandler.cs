using MediatR;
using Application.DTOs;
using AutoMapper;
using Domain.Repositories;
using Domain.Exceptions;

namespace Application.CQRS.Queries
{
    /// <summary>
    /// Handler para obtener una orden por ID
    /// </summary>
    public class GetOrdenByIdQueryHandler : IRequestHandler<GetOrdenByIdQuery, OrdenResponse>
    {
        private readonly IOrdenRepository _repository;
        private readonly IMapper _mapper;

        public GetOrdenByIdQueryHandler(IOrdenRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<OrdenResponse> Handle(GetOrdenByIdQuery request, CancellationToken cancellationToken)
        {
            var orden = await _repository.GetByIdAsync(request.OrdenId);

            if (orden == null)
                throw new NotFoundException($"La orden {request.OrdenId} no existe");

            return _mapper.Map<OrdenResponse>(orden);
        }
    }
}
