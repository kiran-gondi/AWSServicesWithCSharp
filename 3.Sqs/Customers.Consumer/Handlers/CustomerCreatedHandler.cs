using Customers.Consumer.Messages;
using MediatR;

namespace Customers.Consumer.Handlers;

public class CustomerCreatedHandler : IRequestHandler<CustomerCreated>
{
    private readonly ILogger<CustomerCreatedHandler> _logger;

    public CustomerCreatedHandler(ILogger<CustomerCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task<Unit> Handle(CustomerCreated request, CancellationToken cancellationToken)
    {
        // _logger.LogInformation(request.FullName);
        throw new Exception("Something broke oops!");
        return Unit.Task;
    }
}