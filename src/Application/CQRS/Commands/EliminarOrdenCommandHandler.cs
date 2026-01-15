using MediatR;
using Domain.Repositories;
using Domain.Exceptions;

namespace Application.CQRS.Commands
{
    /// <summary>
    /// Handler para eliminar una orden
    /// </summary>
    public class EliminarOrdenCommandHandler : IRequestHandler<EliminarOrdenCommand, bool>
    {
        private readonly IOrdenRepository _repository;

        public EliminarOrdenCommandHandler(IOrdenRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(EliminarOrdenCommand request, CancellationToken cancellationToken)
        {
            var orden = await _repository.GetByIdAsync(request.OrdenId);

            if (orden == null)
                throw new NotFoundException($"La orden {request.OrdenId} no existe");

            var resultado = await _repository.DeleteAsync(request.OrdenId);
            await _repository.SaveChangesAsync();

            return resultado;
        }
    }
}
