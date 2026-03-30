using System.Diagnostics.CodeAnalysis;

namespace EliseuBatista99.Core.Operations
{
    [ExcludeFromCodeCoverage]
    public class OutputMetadataDto : Dto
    {
        public bool? Success { get; set; }

        public List<ErrorDto>? Errors { get; set; }

        public void AddErrors(List<ErrorDto> errors)
        {
            this.Success = false;
            this.Errors = errors;
        }
    }
}
