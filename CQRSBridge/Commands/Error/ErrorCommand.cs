using CQRSBridge.Attribute;
using MediatR;

namespace CQRSBridge.Commands.HelloWorld
{

    [CommandName("Error")]
    public class ErrorCommand : IRequest<Result<EmptyDto>>
    {
      
    }

    public class ErrorCommandHandler : IRequestHandler<ErrorCommand, Result<EmptyDto>>
    {
  
        public async Task<Result<EmptyDto>> Handle(ErrorCommand request, CancellationToken cancellationToken)
        {
            throw new Exception("Error");
        }
    }
}
