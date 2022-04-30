using CQRSBridge.Attribute;
using MediatR;

namespace CQRSBridge.Commands.HelloWorld
{

    [CommandName("HelloWorld")]
    public class HelloWorldCommand : IRequest<Result<HelloWorldDto>>
    {
        public string Echo { get; set; }
    }

    public class HelloWorldCommandHandler : IRequestHandler<HelloWorldCommand, Result<HelloWorldDto>>
    {
  
        public async Task<Result<HelloWorldDto>> Handle(HelloWorldCommand request, CancellationToken cancellationToken)
        {
            return Result<HelloWorldDto>.Success(new HelloWorldDto() { Echo = request.Echo });
        }
    }
}
