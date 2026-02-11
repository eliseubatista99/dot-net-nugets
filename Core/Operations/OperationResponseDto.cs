using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;


namespace EliseuBatista99.Core.Operations
{
    [ExcludeFromCodeCoverage]
    public class OperationResponseDto<TOutput> where TOutput : OperationOutputDto
    {
        public TOutput? Data { get; set; }
        public OutputMetadataDto? Metadata { get; set; }
        public int StatusCode { get; set; }

        public OperationResponseDto()
        {
            StatusCode = StatusCodes.Status200OK;
        }

        public void AddError(ErrorDto error)
        {
            AddErrors(new List<ErrorDto> { error });
        }

        public void AddErrors(List<ErrorDto> errors)
        {
            if (this.Metadata == null)
            {
                this.Metadata = new OutputMetadataDto();
            }

            this.Metadata.AddErrors(errors);
        }

        protected void SetStatusCode(int code)
        {
            StatusCode = code;
        }
    }
}
