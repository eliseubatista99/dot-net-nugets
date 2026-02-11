using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace EliseuBatista99.Core.Operations
{
    public class OperationBase<TInput, TOutput>
        where TInput : OperationInputDto
        where TOutput : OperationOutputDto
    {
        //protected BaseAppController controller;

        protected IExecutionContext ExecutionContext;

        protected IMapper MapperProvider;

        protected TInput? input;

        protected System.Security.Claims.ClaimsPrincipal? User;

        protected OperationResponseDto<TOutput> output;

        public OperationBase(IExecutionContext executionContext)
        {
            //controller = _controller;
            ExecutionContext = executionContext;

            MapperProvider = ExecutionContext.GetService<IMapper>();

            output = new OperationResponseDto<TOutput>
            {
                Metadata = new OutputMetadataDto
                {
                    Success = true,
                    Errors = null,
                }
            };
        }

        protected void SetStatusCode(int code)
        {
            output.StatusCode = code;

            //controller.Response.StatusCode = code;
        }

        protected void SetUser(System.Security.Claims.ClaimsPrincipal _user)
        {
            this.User = _user;

            //controller.Response.StatusCode = code;
        }

        protected virtual void LogOperationExecution()
        {
            var data = System.Text.Json.JsonSerializer.Serialize(new
            {
                Input = this.input,
                //Headers = controller?.Request.Headers,
            }, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

            Console.WriteLine($" Shopping App Server > {this.GetType().Name} > Executing request with > {data}");
        }

        protected virtual void LogOperationResponse()
        {
            var data = System.Text.Json.JsonSerializer.Serialize(new
            {
                Output = this.output,
            }, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

            Console.WriteLine($" Shopping App Server > {this.GetType().Name} > Executing responded with > {data}");
        }

        protected virtual Task HandleExecution()
        {
            return Task.CompletedTask;
        }

        public async Task<OperationResponseDto<TOutput>> Execute()
        {
            LogOperationExecution();

            SetStatusCode(StatusCodes.Status200OK);
            await HandleExecution();

            if (output is null)
            {
                output = new OperationResponseDto<TOutput>
                {
                    Data = default,
                    Metadata = new OutputMetadataDto(),
                };
            }

            LogOperationResponse();

            return output;
        }

        public async Task<TResponse> Execute<TResponse>(System.Security.Claims.ClaimsPrincipal? User)
            where TResponse : OperationResponseDto<TOutput>, new()
        {
            if (User != null)
            {
                SetUser(User);
            }

            var res = await Execute();

            return new TResponse
            {
                Data = res.Data,
                Metadata = res.Metadata,
            };
        }

        public Task<TResponse> Execute<TResponse>(TInput _input, System.Security.Claims.ClaimsPrincipal? User)
            where TResponse : OperationResponseDto<TOutput>, new()
        {
            this.input = _input;

            return Execute<TResponse>(User);
        }
    }
}
