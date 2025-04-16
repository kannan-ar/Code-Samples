using MediatR;

namespace ShoppingApp.Api.Features.Sales.Commands
{
    public record CreateSaleCommand(decimal Amount) : IRequest<Guid>;

    public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, Guid>
    {
        public Task<Guid> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
